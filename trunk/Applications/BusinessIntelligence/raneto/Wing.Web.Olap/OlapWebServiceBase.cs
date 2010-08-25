/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.AgOlap.Controls.General.ClientServer;
using Wing.AgOlap;
using Wing.Olap.Core.Metadata;
using Wing.Olap.Core.Data;
using Wing.Olap.Core.Providers;
using Wing.Olap.Core;
using Wing.Olap.Core.Providers.ClientServer;
using System.IO;
using System.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using Wing.Olap.Core.Storage;
using System.Web;
using System.Configuration;
using Wing.ZipCompression;
using System.Web.Services;
using System.Data;

namespace Wing.Web.Olap
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class OlapWebServiceBase : System.Web.Services.WebService
    {
        public OlapWebServiceBase()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("CompressData"))
            {
                String str = ConfigurationManager.AppSettings["CompressData"];
                try
                {
                    if (!String.IsNullOrEmpty(str))
                        UseCompress = Convert.ToBoolean(str);
                }
                catch (System.FormatException)
                {
                }
            }
        }

        bool UseCompress = true;

        [WebMethod(EnableSession = true)]
        public virtual String PerformOlapServiceAction(String schemaType, String schema)
        {
            DateTime start = DateTime.Now;
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} PerformOlapServiceAction \r\n SCHEMA_TYPE: {1} \r\n SCHEMA: {2} \r\n",
                    DateTime.Now.ToString(), schemaType, schema);

                object type = OlapActionTypes.Parse(typeof(OlapActionTypes), schemaType, true);
                if (type != null)
                {
                    OlapActionTypes actionType = (OlapActionTypes)type;
                    switch (actionType)
                    {
                        case OlapActionTypes.GetMetadata:
                            return GetMetaData(schema);
                        case OlapActionTypes.StorageAction:
                            return PerformStorageAction(schema);
                        case OlapActionTypes.ExportToExcel:
                            return ExportToExcel(schema);
                        case OlapActionTypes.ExecuteQuery:
                            return ExecuteQuery(schema);
                    }
                }

                InvokeResultDescriptor result = new InvokeResultDescriptor();
                return InvokeResultDescriptor.Serialize(result);
            }
            catch (Exception ex)
            {
                InvokeResultDescriptor result = new InvokeResultDescriptor();
                result.Content = ex.ToString();
                result.ContentType = InvokeContentType.Error;
                System.Diagnostics.Trace.TraceError("{0} PerformOlapServiceAction ERROR: {1}\r\n",
                    DateTime.Now.ToString(), ex.ToString());
                return InvokeResultDescriptor.Serialize(result);
            }
            finally
            {
                System.Diagnostics.Trace.TraceInformation("{0} PerformOlapServiceAction: {1} completed \r\n time: {2} \r\n",
                    DateTime.Now.ToString(), schemaType, (DateTime.Now - start).ToString());
            }
        }

        [WebMethod]
        public String About()
        {
            return "Web Service for Visual OLAP Controls Library";
        }

        #region Загрузка метаданных
        String GetMetaData(String schema)
        {
            InvokeResultDescriptor result = new InvokeResultDescriptor();
            String res = null;
            try
            {
                MetadataQuery args = XmlSerializationUtility.XmlStr2Obj<MetadataQuery>(schema);

                if (args != null)
                {
                    switch (args.QueryType)
                    {
                        case MetadataQueryType.GetCubes:
                            res = GetCubes(args);
                            break;
                        case MetadataQueryType.GetMeasures:
                            res = GetMeasures(args);
                            break;
                        case MetadataQueryType.GetKPIs:
                            res = GetKPIs(args);
                            break;
                        case MetadataQueryType.GetLevels:
                            res = GetLevels(args);
                            break;
                        case MetadataQueryType.GetDimensions:
                            res = GetDimensions(args);
                            break;
                        case MetadataQueryType.GetHierarchies:
                            res = GetHierarchies(args);
                            break;
                        case MetadataQueryType.GetDimension:
                            res = GetDimension(args);
                            break;
                        case MetadataQueryType.GetHierarchy:
                            res = GetHierarchy(args);
                            break;
                        case MetadataQueryType.GetMeasureGroups:
                            res = GetMeasureGroups(args);
                            break;
                        case MetadataQueryType.GetLevelProperties:
                            res = GetLevelProperties(args);
                            break;
                        case MetadataQueryType.GetCubeMetadata:
                        case MetadataQueryType.GetCubeMetadata_AllMembers:
                            res = GetCubeMetadata(args);
                            break;
                    }
                }
                result.Content = res;
                if (UseCompress)
                {
                    // Архивация строки
                    String compesed = ZipCompressor.CompressAndConvertToBase64String(res);
                    result.Content = compesed;
                    result.IsArchive = true;
                }
                result.ContentType = InvokeContentType.MultidimData;
            }
            catch (AdomdConnectionException connection_ex)
            {
                result.Content = connection_ex.Message;
                result.ContentType = InvokeContentType.Error;
            }
            catch (AdomdErrorResponseException response_ex)
            {
                result.Content = response_ex.Message;
                result.ContentType = InvokeContentType.Error;
            }
            catch (OlapMetadataResponseException metadata_ex)
            {
                result.Content = metadata_ex.Message;
                result.ContentType = InvokeContentType.Error;
            }
            catch (Exception)
            {
                throw;
            }
            return InvokeResultDescriptor.Serialize(result);
        }

        String GetConnectionString(String connection)
        {
            object str = null;
            if (HttpContext.Current.Session != null)
            {
                str = HttpContext.Current.Session[connection];
            }
            else
            {
                str = this.Application[connection];
            }
            if (str != null)
                return str.ToString();
            else
                throw new Exception(String.Format("Connection: '{0}' not foud in Application state", connection));
        }

        ConnectionInfo GetConnection(String connection)
        {
            object str = null;
            // Если в сессии есть строка соединения, то берем ее. Иначе пытаемся зачитать из Application
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[connection] != null)
            {
                str = HttpContext.Current.Session[connection];
            }
            else
            {
                str = this.Application[connection];
            }
            if (str != null)
                return new ConnectionInfo(connection, str.ToString());
            else
                throw new Exception(String.Format("Connection: '{0}' not foud in Application state", connection));
        }

        String GetLevelProperties(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));

            // Делать коллекцию с ключем "Имя свойства" нельзя, т.к. свойства KEY1, KEY2 и т.д. есть не у всех уровней и например в контроле выбора элемента измерения при построении уловия поиска придется проверять для каких уровней они есть, а для каких нету
            List<LevelPropertyInfo> list = new List<LevelPropertyInfo>();
            if (String.IsNullOrEmpty(args.LevelUniqueName))
            {
                Dictionary<String, LevelInfo> levels = provider.GetLevels(args.CubeName, args.DimensionUniqueName, args.HierarchyUniqueName);
                foreach (LevelInfo li in levels.Values)
                {
                    Dictionary<String, LevelPropertyInfo> properties = provider.GetLevelProperties(args.CubeName,
                        args.DimensionUniqueName,
                        args.HierarchyUniqueName,
                        li.UniqueName);
                    foreach (LevelPropertyInfo pi in properties.Values)
                    {
                        list.Add(pi);
                    }
                }
            }
            else
            {
                Dictionary<string, LevelPropertyInfo> properties = provider.GetLevelProperties(args.CubeName,
                        args.DimensionUniqueName,
                        args.HierarchyUniqueName,
                        args.LevelUniqueName);
                foreach (LevelPropertyInfo pi in properties.Values)
                {
                    list.Add(pi);
                }
            }

            return XmlSerializationUtility.Obj2XmlStr(list, Common.Namespace);
        }

        String GetKPIs(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));

            Dictionary<String, KpiInfo> list = provider.GetKPIs(args.CubeName);

            return XmlSerializationUtility.Obj2XmlStr(list.Values.ToList(), Common.Namespace);
        }

        String GetMeasureGroups(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));

            List<MeasureGroupInfo> list = provider.GetMeasureGroups(args.CubeName);

            return XmlSerializationUtility.Obj2XmlStr(list, Common.Namespace);
        }

        String GetCubeMetadata(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));

            CubeDefInfo info = provider.GetCubeMetadata(args.CubeName, args.QueryType);

            return XmlSerializationUtility.Obj2XmlStr(info, Common.Namespace);
        }

        String GetCubes(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));

            Dictionary<String, CubeDefInfo> list = provider.GetCubes();

            return XmlSerializationUtility.Obj2XmlStr(list.Values.ToList(), Common.Namespace);
        }

        String GetMeasures(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));

            Dictionary<String, MeasureInfo> list = provider.GetMeasures(args.CubeName);

            return XmlSerializationUtility.Obj2XmlStr(list.Values.ToList(), Common.Namespace);
        }

        String GetLevels(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));

            Dictionary<String, LevelInfo> list = provider.GetLevels(args.CubeName, args.DimensionUniqueName, args.HierarchyUniqueName);

            return XmlSerializationUtility.Obj2XmlStr(list.Values.ToList(), Common.Namespace);
        }

        String GetDimensions(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));

            Dictionary<String, DimensionInfo> list = provider.GetDimensions(args.CubeName);

            return XmlSerializationUtility.Obj2XmlStr(list.Values.ToList(), Common.Namespace);
        }

        String GetHierarchies(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));

            Dictionary<String, HierarchyInfo> list = provider.GetHierarchies(args.CubeName, args.DimensionUniqueName);

            return XmlSerializationUtility.Obj2XmlStr(list.Values.ToList(), Common.Namespace);
        }

        String GetDimension(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));

            DimensionInfo info = provider.GetDimension(args.CubeName, args.DimensionUniqueName);
            return XmlSerializationUtility.Obj2XmlStr(info, Common.Namespace);
        }

        String GetHierarchy(MetadataQuery args)
        {
            OlapMetadataProvider provider = new OlapMetadataProvider(new ConnectionInfo(args.Connection, GetConnectionString(args.Connection)));
            HierarchyInfo info = provider.GetHierarchy(args.CubeName, args.DimensionUniqueName, args.HierarchyUniqueName);
            return XmlSerializationUtility.Obj2XmlStr(info, Common.Namespace);
        }
        #endregion Загрузка метаданных

        private String OLAP_DATA_MANAGER = "OLAP_DATA_MANAGER_{0}";

        #region Выполнение запросов
        String ExecuteQuery(String schema)
        {
            InvokeResultDescriptor result = new InvokeResultDescriptor();
            String res = String.Empty;
            try
            {
                String sessionId = String.Empty;
                MdxQueryArgs args = XmlSerializationUtility.XmlStr2Obj<MdxQueryArgs>(schema);
                if (args != null)
                {
                    sessionId = args.SessionId;
                    DefaultQueryExecuter queryExecuter = new DefaultQueryExecuter(GetConnection(args.Connection));
                    if (args.Queries.Count > 0)
                    {
                        result.ContentType = InvokeContentType.MultidimData;

                        switch (args.Type)
                        {
                            case QueryTypes.Update:
                            case QueryTypes.CommitTransaction:
                            case QueryTypes.RollbackTransaction:
                                List<String> results = new List<String>();
                                result.ContentType = InvokeContentType.UpdateResult;
                                foreach (var query in args.Queries)
                                {
                                    try
                                    {
                                        // Method return a value of one (1). 
                                        queryExecuter.ExecuteNonQuery(query, ref sessionId);
                                        results.Add(String.Empty);
                                    }
                                    catch (AdomdConnectionException connection_ex)
                                    {
                                        results.Add(connection_ex.Message);
                                    }
                                    catch (AdomdErrorResponseException response_ex)
                                    {
                                        results.Add(response_ex.Message);
                                    }
                                    catch (AdomdUnknownResponseException unknown_ex)
                                    {
                                        results.Add(unknown_ex.Message);
                                    }
                                    catch (InvalidOperationException invalid_ex)
                                    {
                                        results.Add(invalid_ex.Message);
                                    }
                                }
                                res = XmlSerializationUtility.Obj2XmlStr(results, Common.Namespace);
                                break;
                            case QueryTypes.Select:
                                try
                                {
                                    res = queryExecuter.GetCellSetDescription(args.Queries[0], ref sessionId);
                                }
                                catch (AdomdConnectionException connection_ex)
                                {
                                    res = connection_ex.Message;
                                    result.ContentType = InvokeContentType.Error;
                                }
                                catch (AdomdErrorResponseException response_ex)
                                {
                                    res = response_ex.Message;
                                    result.ContentType = InvokeContentType.Error;
                                }
                                catch (AdomdUnknownResponseException unknown_ex)
                                {
                                    res = unknown_ex.Message;
                                    result.ContentType = InvokeContentType.Error;
                                }
                                catch (InvalidOperationException invalid_ex)
                                {
                                    res = invalid_ex.Message;
                                    result.ContentType = InvokeContentType.Error;
                                }
                                break;
                            case QueryTypes.DrillThrough:
                                try
                                {
                                    var table = queryExecuter.ExecuteReader(args.Queries[0], ref sessionId);
                                    if (table != null)
                                    {
                                        res = XmlSerializationUtility.Obj2XmlStr(DataTableHelper.Create(table));
                                    }
                                }
                                catch (AdomdConnectionException connection_ex)
                                {
                                    res = connection_ex.Message;
                                    result.ContentType = InvokeContentType.Error;
                                }
                                catch (AdomdErrorResponseException response_ex)
                                {
                                    res = response_ex.Message;
                                    result.ContentType = InvokeContentType.Error;
                                }
                                catch (AdomdUnknownResponseException unknown_ex)
                                {
                                    res = unknown_ex.Message;
                                    result.ContentType = InvokeContentType.Error;
                                }
                                catch (InvalidOperationException invalid_ex)
                                {
                                    res = invalid_ex.Message;
                                    result.ContentType = InvokeContentType.Error;
                                }
                                break;
                        }
                    }
                }
                result.Content = res;
                System.Diagnostics.Debug.WriteLine("CellSetData size: " + res.Length);

                if (UseCompress)
                {
                    DateTime start = DateTime.Now;
                    // Архивация строки
                    String compesed = ZipCompressor.CompressAndConvertToBase64String(res);
                    System.Diagnostics.Debug.WriteLine("CellSetData compression time: " + (DateTime.Now - start).ToString());
                    System.Diagnostics.Debug.WriteLine("CellSetData compressed size: " + compesed.Length);

                    result.Content = compesed;
                    result.IsArchive = true;
                }

                result.Headers.Add(new Header(InvokeResultDescriptor.SESSION_ID, sessionId));
                result.Headers.Add(new Header(InvokeResultDescriptor.CONNECTION_ID, args.Connection));
            }
            catch (Exception ex)
            {
                throw;
            }
            return InvokeResultDescriptor.Serialize(result);
        }

        private String ExportToExcel(String schema)
        {
            InvokeResultDescriptor result = new InvokeResultDescriptor();
            String res = String.Empty;
            try
            {
                String sessionId = String.Empty;
                String connection = String.Empty;
                try
                {
                    MdxQueryArgs args = XmlSerializationUtility.XmlStr2Obj<MdxQueryArgs>(schema);
                    if (args != null)
                    {
                        sessionId = args.SessionId;
                        connection = args.Connection;
                        DefaultQueryExecuter queryExecuter = new DefaultQueryExecuter(GetConnection(args.Connection));
                        if (args.Queries.Count > 0)
                        {
                            res = queryExecuter.GetCellSetDescription(args.Queries[0], ref sessionId);
                        }
                    }
                }
                catch (AdomdConnectionException connection_ex)
                {
                    result.Content = connection_ex.Message;
                    result.ContentType = InvokeContentType.Error;
                }
                catch (AdomdErrorResponseException response_ex)
                {
                    result.Content = response_ex.Message;
                    result.ContentType = InvokeContentType.Error;
                }
                catch (AdomdUnknownResponseException unknown_ex)
                {
                    result.Content = unknown_ex.Message;
                    result.ContentType = InvokeContentType.Error;
                }
                catch (InvalidOperationException invalid_ex)
                {
                    result.Content = invalid_ex.Message;
                    result.ContentType = InvokeContentType.Error;
                }

                if (!String.IsNullOrEmpty(res))
                {
                    CellSetData cs = CellSetData.Deserialize(res);
                    PivotDataProvider pivotProvider = new PivotDataProvider(new CellSetDataProvider(cs));
                    res = ExportHelper.ExportToExcel(pivotProvider);
                }

                result.Content = res;
                if (UseCompress)
                {
                    // Архивация строки
                    String compesed = ZipCompressor.CompressAndConvertToBase64String(res);
                    result.Content = compesed;
                    result.IsArchive = true;
                }
                result.ContentType = InvokeContentType.MultidimData;
                result.Headers.Add(new Header(InvokeResultDescriptor.SESSION_ID, sessionId));
                result.Headers.Add(new Header(InvokeResultDescriptor.CONNECTION_ID, connection));
            }
            catch (Exception)
            {
                throw;
            }
            return InvokeResultDescriptor.Serialize(result);
        }
        #endregion Выполнение запросов

        #region Работа с хранилищем
        String PerformStorageAction(String schema)
        {
            InvokeResultDescriptor result = new InvokeResultDescriptor();
            String res = String.Empty;
            try
            {
                StorageActionArgs args = XmlSerializationUtility.XmlStr2Obj<StorageActionArgs>(schema);
                if (args != null)
                {
                    FileStorageProvider storageProvider = new FileStorageProvider();
                    switch (args.ActionType)
                    {
                        case StorageActionTypes.Save:
                            args.FileDescription.ContentFileName = args.FileDescription.Description.Name + ".content." + FileStorageProvider.GetFilteExtension(args.ContentType);
                            storageProvider.Save(HttpContext.Current.User, args.FileDescription.Description.Name + "." + FileStorageProvider.GetFilteExtension(args.ContentType), XmlSerializationUtility.Obj2XmlStr(args.FileDescription));
                            storageProvider.Save(HttpContext.Current.User, args.FileDescription.ContentFileName, args.Content);
                            break;
                        case StorageActionTypes.Load:
                            res = storageProvider.Load(HttpContext.Current.User, args.FileDescription.ContentFileName);
                            break;
                        case StorageActionTypes.GetList:
                            res = XmlSerializationUtility.Obj2XmlStr(storageProvider.GetList(HttpContext.Current.User, "*." + FileStorageProvider.GetFilteExtension(args.ContentType)), Common.Namespace);
                            break;
                        case StorageActionTypes.Clear:
                            res = XmlSerializationUtility.Obj2XmlStr(storageProvider.Clear(HttpContext.Current.User, "*." + FileStorageProvider.GetFilteExtension(args.ContentType)));
                            break;
                        case StorageActionTypes.Delete:
                            storageProvider.Delete(HttpContext.Current.User, args.FileDescription.Description.Name + "." + FileStorageProvider.GetFilteExtension(args.ContentType));
                            storageProvider.Delete(HttpContext.Current.User, args.FileDescription.ContentFileName);
                            res = XmlSerializationUtility.Obj2XmlStr(storageProvider.GetList(HttpContext.Current.User, "*." + FileStorageProvider.GetFilteExtension(args.ContentType)), Common.Namespace);
                            break;
                    }
                }
                result.Content = res;
            }
            catch (Exception)
            {
                throw;
            }
            return InvokeResultDescriptor.Serialize(result);
        }
        #endregion Работа с хранилищем
    }
}

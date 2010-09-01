using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.IO.IsolatedStorage;
using System.ServiceModel;

namespace UILibrary.Olap.UITestApplication
{
    public class Data
    {
        public string OLAPConnectionString { get; set; }

        public string MdxQuery { get; set; }
        public string UpdateScript { get; set; }
        public string MdxDesignerLayout { get; set; }
        public int BackGroundColor { get; set; }


        public Data()
        {
            SetDefault();
        }

        public Data Clone()
        {
            return (Data)MemberwiseClone();
        }
        void SetDefault()
        {
            BackGroundColor = 12;

            // OLAPConnectionString = @"Data Source=.\sql2008;Initial Catalog=Adventure Works DW";
            OLAPConnectionString = string.Empty;

            //OlapWebServiceUrl = new Uri(new Uri(Wing.Olap.Services.ServiceManager.BaseAddress), @"OlapWebService.asmx").AbsoluteUri;

            MdxQuery = @"SELECT
   { [Measures].[Internet Sales Amount]
    , [Measures].[Internet Order Quantity]
    }
  DIMENSION PROPERTIES PARENT_UNIQUE_NAME , HIERARCHY_UNIQUE_NAME , CUSTOM_ROLLUP , UNARY_OPERATOR , KEY0 ON 0
, NON EMPTY ( ( ( DRILLDOWNMEMBER
        ( ( Hierarchize
            ( CrossJoin
              ( [Date].[Calendar].Levels ( 0 ).Members
              , CrossJoin
                ( [Product].[Product Categories].Levels ( 0 ).Members
                , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                )
              )
            )
          +
            DRILLDOWNMEMBER
            ( FILTER
              ( Hierarchize
                ( CrossJoin
                  ( [Date].[Calendar].Levels ( 0 ).Members
                  , CrossJoin
                    ( [Product].[Product Categories].Levels ( 0 ).Members
                    , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                    )
                  )
                )
              , ( [Date].[Calendar].CURRENTMEMBER
                IS
                  [Date].[Calendar].[All Periods]
                )
              )
            , [Product].[Product Categories].[All Products]
            )
          )
        , [Date].[Calendar].[All Periods]
        )
      +
        DRILLDOWNMEMBER
        ( FILTER
          ( DRILLDOWNMEMBER
            ( ( Hierarchize
                ( CrossJoin
                  ( [Date].[Calendar].Levels ( 0 ).Members
                  , CrossJoin
                    ( [Product].[Product Categories].Levels ( 0 ).Members
                    , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                    )
                  )
                )
              +
                DRILLDOWNMEMBER
                ( FILTER
                  ( Hierarchize
                    ( CrossJoin
                      ( [Date].[Calendar].Levels ( 0 ).Members
                      , CrossJoin
                        ( [Product].[Product Categories].Levels ( 0 ).Members
                        , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                        )
                      )
                    )
                  , ( [Date].[Calendar].CURRENTMEMBER
                    IS
                      [Date].[Calendar].[All Periods]
                    )
                  )
                , [Product].[Product Categories].[All Products]
                )
              )
            , [Date].[Calendar].[All Periods]
            )
          , ( ( [Date].[Calendar].CURRENTMEMBER
              , [Product].[Product Categories].CURRENTMEMBER
              )
            IS
              ( [Date].[Calendar].[Calendar Year].&[2002]
              , [Product].[Product Categories].[All Products]
              )
            )
          )
        , [Sales Reason].[Sales Reasons].[All Sales Reasons]
        )
      )
    +
      DRILLDOWNMEMBER
      ( FILTER
        ( ( DRILLDOWNMEMBER
            ( ( Hierarchize
                ( CrossJoin
                  ( [Date].[Calendar].Levels ( 0 ).Members
                  , CrossJoin
                    ( [Product].[Product Categories].Levels ( 0 ).Members
                    , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                    )
                  )
                )
              +
                DRILLDOWNMEMBER
                ( FILTER
                  ( Hierarchize
                    ( CrossJoin
                      ( [Date].[Calendar].Levels ( 0 ).Members
                      , CrossJoin
                        ( [Product].[Product Categories].Levels ( 0 ).Members
                        , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                        )
                      )
                    )
                  , ( [Date].[Calendar].CURRENTMEMBER
                    IS
                      [Date].[Calendar].[All Periods]
                    )
                  )
                , [Product].[Product Categories].[All Products]
                )
              )
            , [Date].[Calendar].[All Periods]
            )
          +
            DRILLDOWNMEMBER
            ( FILTER
              ( DRILLDOWNMEMBER
                ( ( Hierarchize
                    ( CrossJoin
                      ( [Date].[Calendar].Levels ( 0 ).Members
                      , CrossJoin
                        ( [Product].[Product Categories].Levels ( 0 ).Members
                        , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                        )
                      )
                    )
                  +
                    DRILLDOWNMEMBER
                    ( FILTER
                      ( Hierarchize
                        ( CrossJoin
                          ( [Date].[Calendar].Levels ( 0 ).Members
                          , CrossJoin
                            ( [Product].[Product Categories].Levels ( 0 ).Members
                            , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                            )
                          )
                        )
                      , ( [Date].[Calendar].CURRENTMEMBER
                        IS
                          [Date].[Calendar].[All Periods]
                        )
                      )
                    , [Product].[Product Categories].[All Products]
                    )
                  )
                , [Date].[Calendar].[All Periods]
                )
              , ( ( [Date].[Calendar].CURRENTMEMBER
                  , [Product].[Product Categories].CURRENTMEMBER
                  )
                IS
                  ( [Date].[Calendar].[Calendar Year].&[2002]
                  , [Product].[Product Categories].[All Products]
                  )
                )
              )
            , [Sales Reason].[Sales Reasons].[All Sales Reasons]
            )
          )
        , ( ( [Date].[Calendar].CURRENTMEMBER
            , [Product].[Product Categories].CURRENTMEMBER
            )
          IS
            ( [Date].[Calendar].[All Periods]
            , [Product].[Product Categories].[All Products]
            )
          )
        )
      , [Sales Reason].[Sales Reasons].[All Sales Reasons]
      )
    )
  +
    DRILLDOWNMEMBER
    ( FILTER
      ( ( ( DRILLDOWNMEMBER
            ( ( Hierarchize
                ( CrossJoin
                  ( [Date].[Calendar].Levels ( 0 ).Members
                  , CrossJoin
                    ( [Product].[Product Categories].Levels ( 0 ).Members
                    , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                    )
                  )
                )
              +
                DRILLDOWNMEMBER
                ( FILTER
                  ( Hierarchize
                    ( CrossJoin
                      ( [Date].[Calendar].Levels ( 0 ).Members
                      , CrossJoin
                        ( [Product].[Product Categories].Levels ( 0 ).Members
                        , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                        )
                      )
                    )
                  , ( [Date].[Calendar].CURRENTMEMBER
                    IS
                      [Date].[Calendar].[All Periods]
                    )
                  )
                , [Product].[Product Categories].[All Products]
                )
              )
            , [Date].[Calendar].[All Periods]
            )
          +
            DRILLDOWNMEMBER
            ( FILTER
              ( DRILLDOWNMEMBER
                ( ( Hierarchize
                    ( CrossJoin
                      ( [Date].[Calendar].Levels ( 0 ).Members
                      , CrossJoin
                        ( [Product].[Product Categories].Levels ( 0 ).Members
                        , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                        )
                      )
                    )
                  +
                    DRILLDOWNMEMBER
                    ( FILTER
                      ( Hierarchize
                        ( CrossJoin
                          ( [Date].[Calendar].Levels ( 0 ).Members
                          , CrossJoin
                            ( [Product].[Product Categories].Levels ( 0 ).Members
                            , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                            )
                          )
                        )
                      , ( [Date].[Calendar].CURRENTMEMBER
                        IS
                          [Date].[Calendar].[All Periods]
                        )
                      )
                    , [Product].[Product Categories].[All Products]
                    )
                  )
                , [Date].[Calendar].[All Periods]
                )
              , ( ( [Date].[Calendar].CURRENTMEMBER
                  , [Product].[Product Categories].CURRENTMEMBER
                  )
                IS
                  ( [Date].[Calendar].[Calendar Year].&[2002]
                  , [Product].[Product Categories].[All Products]
                  )
                )
              )
            , [Sales Reason].[Sales Reasons].[All Sales Reasons]
            )
          )
        +
          DRILLDOWNMEMBER
          ( FILTER
            ( ( DRILLDOWNMEMBER
                ( ( Hierarchize
                    ( CrossJoin
                      ( [Date].[Calendar].Levels ( 0 ).Members
                      , CrossJoin
                        ( [Product].[Product Categories].Levels ( 0 ).Members
                        , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                        )
                      )
                    )
                  +
                    DRILLDOWNMEMBER
                    ( FILTER
                      ( Hierarchize
                        ( CrossJoin
                          ( [Date].[Calendar].Levels ( 0 ).Members
                          , CrossJoin
                            ( [Product].[Product Categories].Levels ( 0 ).Members
                            , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                            )
                          )
                        )
                      , ( [Date].[Calendar].CURRENTMEMBER
                        IS
                          [Date].[Calendar].[All Periods]
                        )
                      )
                    , [Product].[Product Categories].[All Products]
                    )
                  )
                , [Date].[Calendar].[All Periods]
                )
              +
                DRILLDOWNMEMBER
                ( FILTER
                  ( DRILLDOWNMEMBER
                    ( ( Hierarchize
                        ( CrossJoin
                          ( [Date].[Calendar].Levels ( 0 ).Members
                          , CrossJoin
                            ( [Product].[Product Categories].Levels ( 0 ).Members
                            , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                            )
                          )
                        )
                      +
                        DRILLDOWNMEMBER
                        ( FILTER
                          ( Hierarchize
                            ( CrossJoin
                              ( [Date].[Calendar].Levels ( 0 ).Members
                              , CrossJoin
                                ( [Product].[Product Categories].Levels ( 0 ).Members
                                , [Sales Reason].[Sales Reasons].Levels ( 0 ).Members
                                )
                              )
                            )
                          , ( [Date].[Calendar].CURRENTMEMBER
                            IS
                              [Date].[Calendar].[All Periods]
                            )
                          )
                        , [Product].[Product Categories].[All Products]
                        )
                      )
                    , [Date].[Calendar].[All Periods]
                    )
                  , ( ( [Date].[Calendar].CURRENTMEMBER
                      , [Product].[Product Categories].CURRENTMEMBER
                      )
                    IS
                      ( [Date].[Calendar].[Calendar Year].&[2002]
                      , [Product].[Product Categories].[All Products]
                      )
                    )
                  )
                , [Sales Reason].[Sales Reasons].[All Sales Reasons]
                )
              )
            , ( ( [Date].[Calendar].CURRENTMEMBER
                , [Product].[Product Categories].CURRENTMEMBER
                )
              IS
                ( [Date].[Calendar].[All Periods]
                , [Product].[Product Categories].[All Products]
                )
              )
            )
          , [Sales Reason].[Sales Reasons].[All Sales Reasons]
          )
        )
      , ( [Date].[Calendar].CURRENTMEMBER
        IS
          [Date].[Calendar].[Calendar Year].&[2003]
        )
      )
    , [Product].[Product Categories].[Category].&[1]
    )
  )
  DIMENSION PROPERTIES PARENT_UNIQUE_NAME , HIERARCHY_UNIQUE_NAME , CUSTOM_ROLLUP , UNARY_OPERATOR , KEY0 ON 1
FROM [Adventure Works]
CELL PROPERTIES BACK_COLOR , CELL_ORDINAL , FORE_COLOR , FONT_NAME , FONT_SIZE , FONT_FLAGS , FORMAT_STRING , VALUE , FORMATTED_VALUE , UPDATEABLE
";

            MdxDesignerLayout = @"<?xml version='1.0' encoding='utf-8'?>
<MdxLayoutWrapper xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
  <CubeName>Adventure Works</CubeName>
  <SubCube />
  <Filters />
  <Rows>
    <AreaItemWrapper xsi:type='Hierarchy_AreaItemWrapper'>
      <CustomProperties>
        <PropertyInfo>
          <Name>DIMENSION_CAPTION</Name>
          <Value xsi:type='xsd:string'>Date</Value>
        </PropertyInfo>
        <PropertyInfo>
          <Name>CUBE_CAPTION</Name>
          <Value xsi:type='xsd:string'>Adventure Works</Value>
        </PropertyInfo>
      </CustomProperties>
      <Caption>Date.Calendar</Caption>
      <CompositeFilter>
        <IsUsed>false</IsUsed>
        <MembersFilter>
          <IsUsed>false</IsUsed>
          <SelectedInfo />
          <FilterSet />
        </MembersFilter>
        <Filter>
          <IsUsed>false</IsUsed>
          <CurrentFilter>LabelFilter</CurrentFilter>
          <TopFilter>
            <FilterType>Top</FilterType>
            <Count>10</Count>
            <FilterTarget>Items</FilterTarget>
            <MeasureUniqueName />
          </TopFilter>
          <ValueFilter>
            <FilterType>Equal</FilterType>
            <Num1>0</Num1>
            <Num2>1</Num2>
            <MeasureUniqueName />
          </ValueFilter>
          <LabelFilter>
            <FilterType>Equal</FilterType>
            <Text1 />
            <Text2 />
            <LevelPropertyName />
          </LabelFilter>
        </Filter>
      </CompositeFilter>
      <UniqueName>[Date].[Calendar]</UniqueName>
    </AreaItemWrapper>
    <AreaItemWrapper xsi:type='Hierarchy_AreaItemWrapper'>
      <CustomProperties>
        <PropertyInfo>
          <Name>DIMENSION_CAPTION</Name>
          <Value xsi:type='xsd:string'>Product</Value>
        </PropertyInfo>
        <PropertyInfo>
          <Name>CUBE_CAPTION</Name>
          <Value xsi:type='xsd:string'>Adventure Works</Value>
        </PropertyInfo>
      </CustomProperties>
      <Caption>Product Categories</Caption>
      <CompositeFilter>
        <IsUsed>false</IsUsed>
        <MembersFilter>
          <IsUsed>false</IsUsed>
          <SelectedInfo />
          <FilterSet />
        </MembersFilter>
        <Filter>
          <IsUsed>false</IsUsed>
          <CurrentFilter>LabelFilter</CurrentFilter>
          <TopFilter>
            <FilterType>Top</FilterType>
            <Count>10</Count>
            <FilterTarget>Items</FilterTarget>
            <MeasureUniqueName />
          </TopFilter>
          <ValueFilter>
            <FilterType>Equal</FilterType>
            <Num1>0</Num1>
            <Num2>1</Num2>
            <MeasureUniqueName />
          </ValueFilter>
          <LabelFilter>
            <FilterType>Equal</FilterType>
            <Text1 />
            <Text2 />
            <LevelPropertyName />
          </LabelFilter>
        </Filter>
      </CompositeFilter>
      <UniqueName>[Product].[Product Categories]</UniqueName>
    </AreaItemWrapper>
    <AreaItemWrapper xsi:type='Hierarchy_AreaItemWrapper'>
      <CustomProperties>
        <PropertyInfo>
          <Name>DIMENSION_CAPTION</Name>
          <Value xsi:type='xsd:string'>Sales Reason</Value>
        </PropertyInfo>
        <PropertyInfo>
          <Name>CUBE_CAPTION</Name>
          <Value xsi:type='xsd:string'>Adventure Works</Value>
        </PropertyInfo>
      </CustomProperties>
      <Caption>Sales Reasons</Caption>
      <CompositeFilter>
        <IsUsed>false</IsUsed>
        <MembersFilter>
          <IsUsed>false</IsUsed>
          <SelectedInfo />
          <FilterSet />
        </MembersFilter>
        <Filter>
          <IsUsed>false</IsUsed>
          <CurrentFilter>LabelFilter</CurrentFilter>
          <TopFilter>
            <FilterType>Top</FilterType>
            <Count>10</Count>
            <FilterTarget>Items</FilterTarget>
            <MeasureUniqueName />
          </TopFilter>
          <ValueFilter>
            <FilterType>Equal</FilterType>
            <Num1>0</Num1>
            <Num2>1</Num2>
            <MeasureUniqueName />
          </ValueFilter>
          <LabelFilter>
            <FilterType>Equal</FilterType>
            <Text1 />
            <Text2 />
            <LevelPropertyName />
          </LabelFilter>
        </Filter>
      </CompositeFilter>
      <UniqueName>[Sales Reason].[Sales Reasons]</UniqueName>
    </AreaItemWrapper>
  </Rows>
  <Columns>
    <AreaItemWrapper xsi:type='Values_AreaItemWrapper'>
      <CustomProperties />
      <Caption>Values</Caption>
    </AreaItemWrapper>
  </Columns>
  <Data>
    <AreaItemWrapper xsi:type='Measure_AreaItemWrapper'>
      <CustomProperties />
      <Caption>Internet Sales Amount</Caption>
      <UniqueName>[Measures].[Internet Sales Amount]</UniqueName>
    </AreaItemWrapper>
    <AreaItemWrapper xsi:type='Measure_AreaItemWrapper'>
      <CustomProperties />
      <Caption>Internet Order Quantity</Caption>
      <UniqueName>[Measures].[Internet Order Quantity]</UniqueName>
    </AreaItemWrapper>
  </Data>
  <CalculatedMembers />
  <CalculatedNamedSets />
</MdxLayoutWrapper>
";
            UpdateScript = "--You need to enable WriteBack ability for your cube first.";
        }
    }
    public class Config : INotifyPropertyChanged
    {
        public readonly static Config Default = new Config();
        public readonly static string ConnectionStringId = "OLAPConnectionString";
        public static string OlapWebServiceUrl
        {
            get
            {
                return Wing.Olap.Services.ServiceManager.BaseAddress;
            }
        }

        public static string LastError = "";

        public event PropertyChangedEventHandler PropertyChanged;
        Data m_Data = null;
        public Data Data
        {
            get
            {
                if (m_Data == null)
                    Load();
                return m_Data;
            }
            set
            {
                m_Data = value;
                Refresh();
            }

        }
        public static void Load()
        {
            try
            {
                Default.m_Data = IsolatedStorageSettings.ApplicationSettings["Config"] as Data;
            }
            catch
            {
            }
            try
            {
                if (Default.m_Data == null)
                {
                    Default.m_Data = new Data();
                }
                Default.m_Data = Default.m_Data.Clone();
                Refresh();
            }
            catch (Exception E)
            {
                LastError = E.ToString();
            }
        }

        public static void Save()
        {
            try
            {
                try
                {
                    IsolatedStorageSettings.ApplicationSettings["Config"] = Default.Data;
                }
                catch
                {
                    IsolatedStorageSettings.ApplicationSettings.Add("Config", Default.Data);
                }
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            catch (Exception E)
            {
                LastError = E.ToString();
            }
        }
        public static void SetDefault()
        {
            Default.m_Data = new Data();
            Refresh();
        }
        public static void Refresh()
        {
            if (Default.PropertyChanged != null)
                Default.PropertyChanged(Default, new PropertyChangedEventArgs("Data"));
        }
        public static void Init(string connectionStringId, string connectionString, Action OnSuccess, Action OnError)
        {
            var service = Wing.Olap.Services.ServiceManager.CreateService
             <Wing.Olap.OlapWebService.OlapWebServiceSoapClient
             , Wing.Olap.OlapWebService.OlapWebServiceSoap
             >(OlapWebServiceUrl);

            service.PerformOlapServiceActionCompleted +=
            (object sender
            , Wing.Olap.OlapWebService.PerformOlapServiceActionCompletedEventArgs e
            ) =>
            {
                if (e.Error == null)
                {
                    if (string.IsNullOrEmpty(e.Result))
                    {
                        LastError = "Data service has returned empty value.";
                        if (OnError != null)
                            OnError();
                    }
                    else if (!e.Result.StartsWith(connectionStringId + "="))
                    {
                        LastError = e.Result;
                        if (OnError != null)
                            OnError();
                    }
                    else
                    {
                        Default.Data.OLAPConnectionString = e.Result.Substring(connectionStringId.Length + 1);
                        if (string.IsNullOrEmpty(e.Result))
                        {
                            LastError = "Data service has returned empty connection string.";
                            if (OnError != null)
                                OnError();
                        }
                        else if (OnSuccess != null)
                            OnSuccess();
                    }
                }
                else
                {
                    LastError = e.Error.ToString();
                    if (OnError != null)
                        OnError();
                }
            };
            service.PerformOlapServiceActionAsync("SetConnectionString", connectionStringId + "=" + connectionString);
        }
    }
}

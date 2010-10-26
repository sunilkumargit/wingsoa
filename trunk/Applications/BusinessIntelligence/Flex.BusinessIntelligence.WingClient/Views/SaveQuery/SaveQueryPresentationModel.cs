using System;
using System.ComponentModel.DataAnnotations;
using Flex.BusinessIntelligence.Data;
using Wing.Client.Sdk;

namespace Flex.BusinessIntelligence.WingClient.Views.SaveQuery
{
    public class SaveQueryPresentationModel : ViewPresentationModel
    {
        private CubeQueryInfo _queryInfo;

        public SaveQueryPresentationModel(CubeQueryInfo queryInfo)
        {
            _queryInfo = queryInfo;
        }

        [Display(AutoGenerateField = false)]
        public CubeQueryInfo QueryInfo
        {
            get { return _queryInfo; }
            set { _queryInfo = value; NotifyPropertyChanged("QueryInfo"); }
        }

        [Display(AutoGenerateField = true, Name = "Titulo", Description = "Informe um titulo que descreva esta consulta")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Digite um titulo que identifique a consulta.")]
        public String QueryName
        {
            get { return _queryInfo.Name; }
            set { _queryInfo.Name = value; NotifyPropertyChanged("QueryName"); }
        }
    }
}

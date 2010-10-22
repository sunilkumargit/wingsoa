using System;
using System.ComponentModel.DataAnnotations;
using Flex.BusinessIntelligence.Data;
using Wing.Client.Sdk;

namespace Flex.BusinessIntelligence.WingClient.Views.RegisterCube
{
    public class CubeRegisterPresentationModel : ViewPresentationModel
    {
        private CubeRegistrationInfo _cubeInfo;

        [Display(AutoGenerateField = false)]
        public CubeRegistrationInfo CubeInfo
        {
            get { return _cubeInfo; }
            set { _cubeInfo = value; NotifyPropertyChanged("CubeInfo"); }
        }

        [Display(AutoGenerateField = true, Name = "Servidor")]
        [Required(ErrorMessage = "Informe o caminho completo para o servidor MSAS", AllowEmptyStrings = false)]
        public String ServerName
        {
            get { return _cubeInfo.ServerName; }
            set { _cubeInfo.ServerName = value; NotifyPropertyChanged("ServerName"); }
        }

        [Display(AutoGenerateField = true, Name = "Usuário")]
        [Required(ErrorMessage = "Informe o nome do usuário no servidor MSAS", AllowEmptyStrings = false)]
        public String UserName
        {
            get { return _cubeInfo.UserName; }
            set { _cubeInfo.UserName = value; NotifyPropertyChanged("UserName"); }
        }

        [Display(AutoGenerateField = true, Name = "Senha", Description = "Senha no servidor MSAS")]
        [Required(ErrorMessage = "Informe a senha para o servidor MSAS", AllowEmptyStrings = false)]
        public String Password
        {
            get { return _cubeInfo.Password; }
            set { _cubeInfo.Password = value; NotifyPropertyChanged("Password"); }
        }

        [Display(AutoGenerateField = true, Name = "Banco de dados", Description = "Nome do banco de dados no servidor MSAS")]
        [Required(ErrorMessage = "Informe o nome do banco de dados", AllowEmptyStrings = false)]
        public String CatalogName
        {
            get { return _cubeInfo.CatalogName; }
            set { _cubeInfo.CatalogName = value; NotifyPropertyChanged("CatalogName"); }
        }

        [Display(AutoGenerateField = true, Name = "Cubo", Description = "Nome do cubo")]
        [Required(ErrorMessage = "Informe o nome do cubo no servidor MSAS", AllowEmptyStrings = false)]
        public String CubeName
        {
            get { return _cubeInfo.CubeName; }
            set { _cubeInfo.CubeName = value; NotifyPropertyChanged("CubeName"); }
        }

        [Display(AutoGenerateField = true, Name = "Descrição", Description = "Informe uma descrição que identifique este cubo para o usuário final")]
        [Required(ErrorMessage = "Informe uma descrição para o cubo", AllowEmptyStrings = false)]
        public String Description
        {
            get { return _cubeInfo.Description; }
            set { _cubeInfo.Description = value; NotifyPropertyChanged("Description"); }
        }
    }
}

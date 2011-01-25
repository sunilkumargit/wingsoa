using System;
using System.Collections.Generic;
using System.Linq;

using System.Collections.ObjectModel;

namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Classe base para a criação de controles html.
    /// </summary>
    /// <remarks>
    /// A classe HtmlObject provê as funcionalidades básicas para
    /// o desenvolvimento de controles HTML. Ao criar um controle
    /// que herde da classe HtmlObject você terá acesso a um sistema
    /// de renderização do controle e a um sistema de armazenamento e 
    /// vinculação de dados (binding).
    /// <para>
    /// Para saber mais sobre o sistema de propriedades e vinculação de dados,
    /// consulte as classes <see cref="ControlProperty"/> e <see cref="Binding"/>
    /// </para>
    /// </remarks>
    [System.Diagnostics.DebuggerDisplay("{UnderlyingTypeName}#{Id} ({TagName})")]
    public abstract class HtmlObject : IControlInstanceAdapter
    {
        /// <summary>
        /// Identifica uma instancia do controle. O valor dessa propriedade também
        /// será renderizada como o atributo html "id".
        /// </summary>
        public static readonly ControlProperty IdProperty = ControlProperty.Register("Id",
            typeof(String),
            typeof(HtmlObject),
            new HtmlAttributePropertyApplier(HtmlAttr.Id));

        /// <summary>
        /// Tag que representa o controle no documento html.
        /// </summary>
        public static readonly ControlProperty TagProperty = ControlProperty.Register("Tag",
            typeof(HtmlTag),
            typeof(HtmlObject),
            null,
            HtmlTag.Unknown);

        /// <summary>
        /// O conteúdo desta propriedade é gravado no stram de saída sem alterações.
        /// Um exemplo do uso desta propriedade é gravar fragmentos HTML na saída.
        /// </summary>
        /// <remarks>
        /// Essa propriedade difere da propriedade <see cref="HtmlObject.TextProperty"/> no modo
        /// de saída. Enquanto a propriedadade <see cref="HtmlObject.TextProperty"/> altera a saída
        /// para que seja compativel com um documento HTML (por exemplo, conversão de acentos para códigos HTML, etc.),
        /// a propriedade <see cref="HtmlObject.RawInnerTextProperty"/> terá sua saída gravada sem
        /// que essas conversões sejam feitas.
        /// </remarks>
        public static readonly ControlProperty RawInnerTextProperty = ControlProperty.Register("RawInnerText",
            typeof(String),
            typeof(HtmlObject),
            new TextContentApplier(true));

        /// <summary>
        /// A propriedade DataContext mantém o objeto que será usado como fonte de dados no sistema de vinculação
        /// de dados (binding).
        /// </summary>
        /// <remarks>
        /// A propriedade <see cref="HtmlObject.DataContextProperty"/> é uma propriedade especial. Elá mantém o objeto
        /// que será usado como fonte de dados para que a vinculação de dados (binding) em outras propriedades seja feita.
        /// A fazer uma vinculação de dados (binding) em outra propriedade, o sistema de binding irá procurar na propriedade
        /// DataContext o objeto que será usado para resolver a vinculação de dados. Se o valor de DataContext do objeto atual for
        /// null, então a busca por um objeto continua sucessivamente no controle pai do controle atual, até que um DataContext
        /// com um valor diferente de null seja encontrado ou todos os controles pai sejam consultados.
        /// <para>
        /// Para mais informações sobre o sistema de vinculação de dados, consulte a classe <see cref="Binding"/>.
        /// </para>
        /// </remarks>
        public static readonly ControlProperty DataContextProperty = ControlProperty.Register("DataContext",
            typeof(Object),
            typeof(HtmlObject),
            null);

        /// <summary>
        /// Ativa ou desativa a gravação da saída do controle no stream de saída. 
        /// </summary>
        /// <remarks>
        /// A propriedade <see cref="HtmlObject.RenderEnabledProperty"/> ativa ou desativa a renderização do controle
        /// no stream de saída. Se o valor for 'false', o controle e seus filhos não serão processados.
        /// No entando, os eventos <see cref="HtmlObject.DataBindEvent"/> e <see cref="HtmlObject.PreRenderEvent"/> serão
        /// invocados no controle principal (mas não no filhos do controle desativado).
        /// </remarks>
        public static readonly ControlProperty RenderEnabledProperty = ControlProperty.Register("RenderEnabled",
            typeof(bool),
            typeof(HtmlObject),
            null,
            true);


        /// <summary>
        /// Texto que será gravado como saída dentro da tag do controle.
        /// </summary>
        /// <remarks>
        /// O valor da propriedade <see cref="HtmlObject.TextProperty"/> será gravado no stream de saída de forma que
        /// seja compatível com um documento HTML. Dessa forma, acentos e caracteres especiais serão convertidos para códigos
        /// html. Por exemplo: a letra á será gravada como acute;a, assim como qualquer caractere que não seja compativel
        /// com um documento HTMl.
        /// </remarks>
        public static readonly ControlProperty TextProperty = ControlProperty.Register("Text",
            typeof(String),
            typeof(HtmlObject),
            new TextContentApplier());

        public static readonly ControlProperty InstanceIdProperty = ControlProperty.Register("InstanceId",
            typeof(String),
            typeof(HtmlObject),
            new HtmlCustomAttributePropertyApplier("jqui-inst", ""), "");

        /// <summary>
        /// Evento disparado antes da vinculação dos dados às propriedades.
        /// </summary>
        public event HtmlObjectDelegate DataBindEvent;

        /// <summary>
        /// Evento disparado antes do inicio da renderização do controle. 
        /// </summary>
        public event HtmlObjectDelegate PreRenderEvent;

        /// <summary>
        /// Evento disparado após a tag inicial do controle ter sido gravada na saída e antes do fechamento tag ter sido gravada.
        /// </summary>
        /// <remarks>
        /// O evento <see cref="HtmlObject.RenderContentEvent"/> pode ser usado para gravar conteúdo customizado dentro da tag de um controle.
        /// </remarks>
        /// <example>
        /// Este exemplo mostra como gravar uma tag span dentro do controle <see cref="PanelControl"/>.
        /// <code>
        /// 
        /// // create a new instance of panel control.
        /// var panel = new PanelControl();
        /// 
        /// // subscribe on DataBindEvent.
        /// panel.DataBindEvent += (panel) => {
        ///     // get the current renderization context
        ///     var ctx = panel.CurrentContext;
        ///     
        ///     // add an attribute to span tag
        ///     ctx.Document.AddAttribute("class", "my-custom-class");
        ///     
        ///     // render the span tag.
        ///     ctx.Document.RenderBeginTag("span");
        ///     
        ///     // write content.
        ///     ctx.Document.Write("this is a text inside a span tag");
        ///     
        ///     // close the tag.
        ///     ctx.Document.RenderEndTag();
        /// };
        /// </code>
        /// </example>
        public event HtmlObjectDelegate RenderContentEvent;

        /// <summary>
        /// O evento PostEventEvent é disparado após o controle e seus filhos terem sido renderizados.
        /// </summary>
        public event HtmlObjectDelegate PostRenderEvent;

        #region private fields
        // properties values  store
        private Dictionary<ControlProperty, ControlPropertySlot> _slots;

        // property bindings
        private List<ControlPropertySlot> _bindings;

        // html attributes
        private HtmlAttributeCollection _attributes;

        // html css classes
        private ObservableCollection<string> _cssClasses;

        // css properties
        private HtmlStyleCollection _styles;

        // render process result
        private ControlPropertyApplyResult _renderResult;

        // template property bindings 
        private List<ControlPropertyTemplateBinding> _templateBindings;

        // property with [TemplatePart] attribute
        private List<ControlProperty> _templatedProperties;
        #endregion

        /// <summary>
        /// Cria uma nova instancia de <see cref="HtmlObject"/>
        /// </summary>
        /// <param name="tag">Tag html que representa o controle.</param>
        public HtmlObject(HtmlTag tag)
        {
            _slots = new Dictionary<ControlProperty, ControlPropertySlot>();
            _renderResult = new ControlPropertyApplyResult(this);
            _templatedProperties = ControlProperty.GetTemplatedProperties(this.GetType());
            Tag = tag;
        }

        /// <summary>
        /// Atribui um valor para uma propriedade.
        /// </summary>
        /// <param name="property">Propriedade que terá seu valor alterado</param>
        /// <param name="value">Novo valor da propriedade</param>
        public void SetValue(ControlProperty property, Object value)
        {
            var slot = GetSlot(property, true);
            if (slot.StoredValue == value)
                return;
            var oldValue = slot.StoredValue;
            slot.StoredValue = value;
            property.NotifyPropertyChanged(this, oldValue, slot.StoredValue);
        }

        /// <summary>
        /// Atribui um valor para uma propriedade somente-leitura.
        /// </summary>
        /// <param name="property">Propriedade que terá seu valor alterado.</param>
        /// <param name="value">Novo valor da propriedade</param>
        protected void SetReadOnlyValue(ControlProperty property, Object value)
        {
            if (!property.IsReadOnly)
                throw new InvalidOperationException(String.Format("The property {0}.{1} is not a readonly property.", property.OwnerTypeName, property.PropertyTypeName));
            var slot = GetSlot(property, true);
            if (slot.StoredValue == value)
                return;
            var oldValue = slot.StoredValue;
            slot.SetReadOnlyValue(value);
            property.NotifyPropertyChanged(this, oldValue, slot.StoredValue);
        }

        /// <summary>
        /// Retorna o valor de uma propriedade.
        /// </summary>
        /// <param name="property">Propriedade requisitada.</param>
        /// <returns>Retorna o valor corrente da propriedade ou seu valor default definido no momento do registro.</returns>
        public Object GetValue(ControlProperty property)
        {
            var slot = GetSlot(property, false);
            return slot == null ? property.DefaultValue : slot.CurrentValue;
        }

        /// <summary>
        /// Retorna o valor de uma propriedade.
        /// </summary>
        /// <typeparam name="T">Tipo esperado da propriedade. Se o tipo especificado for diferente do tipo armazenado, uma exceção será levantada.</typeparam>
        /// <param name="property">Propriedade requisitada.</param>
        /// <returns>Retorna o valor corrente da propriedade ou seu valor default definido no momento do registro.</returns>
        /// <exception cref="System.InvalidCastException">Caso o tipo especificado em "T" não seja compatível com o tipo do dado armazenado.</exception>
        public T GetValue<T>(ControlProperty property)
        {
            return (T)GetValue(property);
        }


        /// <summary>
        /// Indica se o controle já foi inicializado.
        /// Um controle é inicializado apenas uma vez, independente 
        /// de quantas vezes ele é renderizado.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Atalho para <see cref="HtmlObject.IdProperty"/>.
        /// </summary>
        public String Id
        {
            get { return GetValue<String>(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        /// <summary>
        /// Atalho para <see cref="HtmlObject.TagProperty"/>.
        /// </summary>
        public HtmlTag Tag
        {
            get { return GetValue<HtmlTag>(TagProperty); }
            set { SetValue(TagProperty, value); }
        }

        /// <summary>
        /// Atalho para <see cref="HtmlObject.RenderEnabledProperty"/>.
        /// </summary>
        public bool RenderEnabled
        {
            get { return GetValue<bool>(RenderEnabledProperty); }
            set { SetValue(RenderEnabledProperty, value); }
        }

        /// <summary>
        /// Atalho para <see cref="HtmlObject.TextProperty"/>
        /// </summary>
        public String Text
        {
            get { return GetValue<String>(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public String RawInnerText
        {
            get { return GetValue<String>(RawInnerTextProperty); }
            set { SetValue(RawInnerTextProperty, value); }
        }

        /// <summary>
        /// Lista de classes css que serão renderizadas no controle.
        /// </summary>
        public ObservableCollection<String> CssClasses
        {
            get { return _cssClasses ?? (_cssClasses = new ObservableCollection<string>()); }

        }

        /// <summary>
        /// Indica se o controle possui ou não uma classe css.
        /// </summary>
        /// <param name="cssClass">Nome da classe css.</param>
        /// <returns>'true' caso a classe css conste na lista de classes do controle.</returns>
        public bool HasClass(String cssClass)
        {
            return _cssClasses != null && _cssClasses.Any(i => cssClass.EqualsIgnoreCase(i));
        }

        /// <summary>
        /// Atributes HTML que serão renderizados no controle.
        /// </summary>
        public HtmlAttributeCollection Attributes
        {
            get
            {
                if (_attributes == null)
                    _attributes = new HtmlAttributeCollection();
                return _attributes;
            }
        }

        /// <summary>
        /// Propriedades css que serão renderizadas no atribute "style" do controle.
        /// </summary>
        public HtmlStyleCollection Styles
        {
            get
            {
                if (_styles == null)
                    _styles = new HtmlStyleCollection();
                return _styles;
            }
        }

        /// <summary>
        /// Atalho para <see cref="HtmlObject.DataContextProperty"/>
        /// </summary>
        public Object DataContext
        {
            get { return GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        /// <summary>
        /// Retorna o nome da tag que será usada para renderizar o controle.
        /// </summary>
        /// <returns>O nome da tag ou null caso a tag não deva ser renderizada.</returns>
        protected virtual String GetTagName()
        {
            return (Tag == HtmlTag.Unknown ? null : Tag.ToString().ToLower());
        }


        /// <summary>
        /// Contexto de renderização corrente.
        /// </summary>
        public RenderContext CurrentContext { get; internal set; }

        /// <summary>
        /// Objeto do contexto de dados.
        /// Está propriedade difere da propridade <see cref="HtmlObject.DataContextProperty"/>.
        /// Enquanto a propriedade <see cref="HtmlObject.DataContextProperty"/> contém o valor
        /// do contexto de dados atribuido diretamente ao controle, a propriedade <see cref="HtmlObject.DataItem"/>
        /// contém o objeto que foi atribuído diretamente ao controle ou o contexto do controle pai
        /// caso a propriedade <see cref="HtmlObject.DataContextProperty"/> seja null.
        /// </summary>
        public Object DataItem { get { return DataContext ?? CurrentContext.CurrentDataItem; } }

        /// <summary>
        /// Renderiza o controle e seus filhos em um contexto.
        /// </summary>
        /// <param name="ctx">Contexto onde o controle deve ser renderizado.</param>
        public void Render(RenderContext ctx)
        {
            ctx.PushControl(this);
            InitInternal(ctx);
            ctx._renderizations++;

            DataBind();

            if (this is IExtensible)
                ControlFactory.ExtendControl(this, ExtensionStage.PreRender);

            PreRender();

            if (this is IExtensible)
                ControlFactory.ExtendControl(this, ExtensionStage.Render);

            if (RenderEnabled)
            {
                if (_templatedProperties.Count > 0)
                {
                    foreach (var property in _templatedProperties)
                    {
                        var control = this.GetValue(property) as HtmlObject;
                        if (control != null)
                            control.ApplyTemplate(this);
                    }
                }
                ApplyProperties();
                var tagName = GetTagName();
                if (tagName.HasValue())
                    RenderBeginTag(tagName, _renderResult.Attributes);
                RenderContent(_renderResult.InnerText.ToString(), _renderResult.RawInnerText.ToString());
                if (tagName.HasValue())
                    RenderEndTag();
                PostRender();
                if (this is IExtensible)
                    ControlFactory.ExtendControl(this, ExtensionStage.PostRender);
            }

            ctx.PopControl();
        }

        public virtual void ApplyTemplate(HtmlObject source)
        {
            InitInternal(source.CurrentContext);
            if (this._templateBindings != null && this._templateBindings.Count > 0)
            {
                foreach (var binding in _templateBindings)
                {
                    var slot = source.GetSlot(binding.SourceProperty, true);
                    slot.LastTemplateBindingSessionId = CurrentContext.RenderSessionId;
                    this.SetValue(binding.TargetProperty, slot.CurrentValue);
                }
            }
        }

        /// <summary>
        /// Sobreponha este método para realizar inicialização no controle.
        /// Este método é invocado apenas 1 vez, independente de quantas
        /// vezes o controle é renderizado, no mesmo ou em outro <see cref="RenderContext"/>
        /// </summary>
        protected virtual void Init() { }

        /// <summary>
        /// Sobreponha este método para customizar a vinculação de dados
        /// </summary>
        protected virtual void DataBind()
        {
            if (DataBindEvent != null)
                DataBindEvent.Invoke(this);
            PerformBindinds();
        }

        /// <summary>
        /// Sobreponha este método para realizar tarefas no controle antes que ele seja renderizado.
        /// Este método é invocado depois de <see cref="HtmlObject.DataBind"/>.
        /// </summary>
        protected virtual void PreRender()
        {
            if (PreRenderEvent != null)
                PreRenderEvent.Invoke(this);
        }

        /// <summary>
        /// Sobreponha este método para customizar a renderização do inicio da tag do controle.
        /// Este método não é invocado caso <see cref="HtmlObject.TagProperty"/> seja igual a <see cref="HtmlTag.Unknown"/>
        /// ou <see cref="HtmlObject.GetTagName"/> retornar null.
        /// </summary>
        /// <param name="tagName">Nome da tag</param>
        /// <param name="attributes">Attributos que devem ser renderizados na tag inicial.</param>
        protected virtual void RenderBeginTag(String tagName, HtmlAttributeCollection attributes)
        {
            if (attributes.Count > 0)
            {
                foreach (var attr in attributes)
                    CurrentContext.Document.AddAttribute(attr.Name, attr.Value);
            }
            CurrentContext.Document.RenderBeginTag(tagName);
        }

        /// <summary>
        /// Sobreponha este método para customizar a renderização do conteúdo
        /// do controle. Ele é invocado após <see cref="HtmlObject.RenderBeginTag"/>
        /// </summary>
        /// <param name="innerText">Texto que deve ser renderizado no conteúdo</param>
        /// <param name="rawInnerText"></param>
        protected virtual void RenderContent(String innerText, String rawInnerText)
        {
            if (innerText.HasValue())
                CurrentContext.Document.WriteEncodedText(innerText);
            if (rawInnerText.HasValue())
                CurrentContext.Document.Write(rawInnerText);

            if (RenderContentEvent != null)
                RenderContentEvent.Invoke(this);
        }


        /// <summary>
        /// Sobreponha este método para customizar a renderização da tag de fechamento
        /// ou para incluir conteúdo no controle antes que a tag seja fechada.
        /// </summary>
        protected virtual void RenderEndTag()
        {
            CurrentContext.Document.RenderEndTag();
        }

        /// <summary>
        /// Registra uma vinculação de dados a uma propriedade do controle.
        /// </summary>
        /// <param name="targetProperty">Propriedade alvo da vinculação de dados. É a propriedade que receberá o valor resolvido.</param>
        /// <param name="binding">Informações sobre a vinculação de dados.</param>
        public void PropertyBinding(ControlProperty targetProperty, Binding binding)
        {
            var slot = GetSlot(targetProperty, true);
            slot.Binding = binding;
            if (binding == null && _bindings != null)
                _bindings.Remove(slot);
            else
            {
                _bindings = _bindings ?? new List<ControlPropertySlot>();
                _bindings.AddIfNotExists(slot);
            }
        }

        /// <summary>
        /// Registra uma vinculação de propriedades 'template'. 
        /// </summary>
        /// <param name="targetProperty">Propriedade alvo. É a propriedade que irá receber o valor resolvido do binding.</param>
        /// <param name="sourceProperty">Propriedade fonte. É a propriedade no controle que detém a propriedade 'template'. O valor dessa propriedadade
        /// é repassada para <paramref name="targetProperty"/> e ela não é renderizada no controle fonte.</param>
        public void TemplateBinding(ControlProperty targetProperty, ControlProperty sourceProperty)
        {
            _templateBindings = _templateBindings ?? new List<ControlPropertyTemplateBinding>();
            _templateBindings.Add(new ControlPropertyTemplateBinding(targetProperty, sourceProperty));
        }

        /// <summary>
        /// Altera o aplicador de uma propriedade para a instancia do controle.
        /// Apenas a instancia atual é afetada.
        /// </summary>
        /// <param name="property">Propriedade que terá seu aplicador alterado.</param>
        /// <param name="applier">Aplicador customizado.</param>
        void IControlInstanceAdapter.SetCustomApplier(ControlProperty property, IControlPropertyApplier applier)
        {
            var slot = GetSlot(property, true);
            slot.CustomApplier = applier;
        }

        void IControlInstanceAdapter.IgnoreApplier(params ControlProperty[] properties)
        {
            if (properties == null)
                return;
            foreach (var property in properties)
            {
                var slot = GetSlot(property, true);
                slot.CustomApplier = new EmptyPropertyApplier();
            }
        }

        /// <summary>
        /// Classe usada para substituir um applier ignorado.
        /// </summary>
        private class EmptyPropertyApplier : IControlPropertyApplier
        {
            public void ApplyProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
            {
            }
        }

        /// <summary>
        /// Retorna o customizador para a instancia do controle atual.
        /// </summary>
        /// <returns></returns>
        public IControlInstanceAdapter GetAdapter()
        {
            return this;
        }


        #region Privates and Internals
        private ControlPropertySlot GetSlot(ControlProperty property, bool createIfNotExists)
        {
            ControlPropertySlot result = null;
            if (!_slots.TryGetValue(property, out result) && createIfNotExists)
            {
                result = new ControlPropertySlot(this, property);
                _slots[property] = result;
            }
            return result;
        }

        internal void InitInternal(RenderContext ctx)
        {
            CurrentContext = ctx;
            if (IsInitialized)
                return;
            // iniciar o controle
            this.IsInitialized = true;
            if (this is IExtensible)
                ControlFactory.ExtendControl(this, ExtensionStage.PreInit);
            Init();
            if (this is IExtensible)
                ControlFactory.ExtendControl(this, ExtensionStage.PostInit);
            ControlFactory.PerformExtensionDelegates(this);
        }

        private void PerformBindinds()
        {
            if (_bindings != null)
            {
                foreach (var slot in _bindings)
                    slot.Binding.Resolve(this, slot.Property);
            }
        }

        private void ApplyProperties()
        {
            _renderResult.Begin();
            var propertyList = ControlProperty.GetPropertiesWithAppliersForType(this.GetType());

            foreach (var property in propertyList)
            {
                var slot = GetSlot(property, false);
                if (slot == null
                    && property.Applier != null)
                    property.Applier.ApplyProperty(this, property, property.DefaultValue, _renderResult);

                else if (slot != null
                    && slot.CurrentApplier != null
                    && slot.LastTemplateBindingSessionId != CurrentContext.RenderSessionId)
                    slot.CurrentApplier.ApplyProperty(this, property, slot.CurrentValue, _renderResult);
            }

            if (_styles != null)
            {
                foreach (var item in _styles)
                    _renderResult.Styles[item.Style] = item.Value;
            }

            if (_attributes != null)
            {
                foreach (var item in _attributes)
                    _renderResult.Attributes[item.Name] = item.Value;
            }

            if (_cssClasses != null)
            {
                var cssClass = _renderResult.Attributes[HtmlAttr.Class];
                if (cssClass.HasValue())
                {
                    var current = cssClass.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var cssItem in _cssClasses)
                        current.AddIfNotExists(cssItem);

                    cssClass = "";
                    foreach (var cssItem in current)
                        cssClass += cssItem + " ";
                    _renderResult.Attributes[HtmlAttr.Class] = cssClass.Trim();
                }
                else
                {
                    foreach (var cssItem in _cssClasses)
                        cssClass += cssItem + " ";
                    _renderResult.Attributes[HtmlAttr.Class] = cssClass.Trim();
                }
            }

            if (_renderResult.Styles.Count > 0)
                _renderResult.Attributes[HtmlAttr.Style] = _renderResult.Styles.ToStyleString();
        }

        protected virtual void PostRender()
        {
            if (PostRenderEvent != null)
                PostRenderEvent.Invoke(this);
        }

        public static string CreateUniqueId()
        {
            return CodeHelper.UniqueName("ho");
        }

        #endregion

        public String UnderlyingTypeName { get { return GetType().Name; } }
        public String TagName { get { return GetTagName(); } }

        public void CheckId()
        {
            if (this.Id.IsEmpty())
                this.Id = CreateUniqueId();
        }

        protected void AddControlToCurrentContext(HtmlObject obj)
        {
            obj.InitInternal(CurrentContext);
        }

        public String InstanceId
        {
            get { return GetValue<String>(InstanceIdProperty); }
            set { SetValue(InstanceIdProperty, value); }
        }

        public void SetInstanceId(bool reset = false)
        {
            if (InstanceId.IsEmpty() || reset)
                InstanceId = CodeHelper.UniqueName("x");
        }
    }
}

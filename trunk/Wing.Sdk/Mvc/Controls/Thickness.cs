using System;
using System.ComponentModel;

namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Representa um conjunto de coordenadas Top, Left, Bootom, Right em uma coordenada css.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class Thickness : INotifyPropertyChanged
    {
        private int? _top;
        private int? _left;
        private int? _right;
        private int? _bottom;
        private CssUnit _unit;

        /// <summary>
        /// Coordenada superior. Pode ser null.
        /// </summary>
        public int? Top
        {
            get { return _top; }
            set
            {
                if (_top == value)
                    return;
                _top = value;
                NotifyPropertyChanged("Top");
            }
        }

        /// <summary>
        /// Coordenada inferior. Pode ser null.
        /// </summary>
        public int? Bottom
        {
            get { return _bottom; }
            set
            {
                if (_bottom == value)
                    return;
                _bottom = value;
                NotifyPropertyChanged("Bottom");
            }
        }

        /// <summary>
        /// Coordenada à esquerda. Pode ser null.
        /// </summary>
        public int? Left
        {
            get
            { return _left; }
            set
            {
                if (_left == value)
                    return;
                _left = value;
                NotifyPropertyChanged("Left");
            }
        }

        /// <summary>
        /// Coordenada à direita. Pode ser null.
        /// </summary>
        public int? Right
        {
            get { return _right; }
            set
            {
                if (_right == value)
                    return;
                _right = value;
                NotifyPropertyChanged("Right");
            }
        }

        /// <summary>
        /// Unidade css que será gravada na saída.
        /// </summary>
        public CssUnit Unit
        {
            get { return _unit; }
            set
            {
                if (_unit == value)
                    return;
                _unit = value;
                NotifyPropertyChanged("Unit");
            }
        }

        /// <summary>
        /// Cria uma nova instancia de <see cref="Thickness"/> com todos os valores nulos.
        /// </summary>
        /// <param name="unit">Unidade css a utilizar</param>
        public Thickness(CssUnit unit = CssUnit.Px)
        {
            Unit = unit;
        }

        /// <summary>
        /// Atribui o mesmo valor a todas as coordenadas.
        /// </summary>
        /// <param name="all">Valor a ser atribuido.</param>
        /// <param name="unit">Unidade a ser utilizada.</param>
        public void All(int all, CssUnit unit = CssUnit.Px)
        {
            Top = Bottom = Left = Right = all;
            Unit = unit;
        }

        /// <summary>
        /// Cria uma nova instancia de <see cref="Thickness"/>.
        /// </summary>
        /// <param name="all">Valor que será atribuído a todas coordenadas.</param>
        /// <param name="unit">Unidade a ser utilizada.</param>
        public Thickness(int all, CssUnit unit = CssUnit.Px)
        {
            Top = Bottom = Left = Right = all;
            Unit = unit;
        }

        /// <summary>
        /// Cria uma nova instancia de <see cref="Thickness"/>.
        /// </summary>
        /// <param name="v">Valor a ser atribuido às propriedades Top e Bottom</param>
        /// <param name="h">Valor a ser atribuido às propriedades Left e Right</param>
        /// <param name="unit">Unidade a ser utilizada.</param>
        public Thickness(int v, int? h, CssUnit unit = CssUnit.Px)
        {
            Top = Bottom = v;
            Left = Right = h;
            Unit = unit;
        }

        /// <summary>
        /// Cria uma nova instancia de <see cref="Thickness"/>.
        /// </summary>
        /// <param name="top">Valor da coordenada superior.</param>
        /// <param name="right">Valor da coordenada direita.</param>
        /// <param name="bottom">Valor da coordenada inferior.</param>
        /// <param name="left">Valor da coordenada esquerda.</param>
        /// <param name="unit">Unidade css a ser utilizada.</param>
        public Thickness(int? top, int? right, int? bottom, int? left, CssUnit unit = CssUnit.Px)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
            Unit = unit;
        }

        /// <summary>
        /// Retorna 'true' caso alguma coordenada possuir um valor diferente de null./>
        /// </summary>
        public bool HasValue
        {
            get { return IsSetted(); }
        }

        /// <summary>
        /// Retorna 'true' se os valores das coordenadas e da unidade css for igual aos valores
        /// do objeto recebido como parametro.
        /// </summary>
        /// <param name="obj">Objeto a comparar</param>
        /// <returns>'true' se a coordenadas e unidades forem iguais nos dois objetos.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            Thickness o = (Thickness)obj;
            return o.Top == this.Top
                && o.Left == this.Left
                && o.Bottom == this.Bottom
                && o.Right == this.Right
                && o.Unit == this.Unit;
        }

        /// <summary>
        /// Retorna uma string no formato valor + unidade que represente a coordenada superior.
        /// </summary>
        /// <returns>String no formato css</returns>
        public string GetTopStringValue()
        {
            return Top.HasValue ? String.Format("{0}{1}", Top.Value, Unit.ToString().ToLower()) : "";
        }

        /// <summary>
        /// Retorna uma string no formato valor + unidade que represente a coordenada inferior.
        /// </summary>
        /// <returns>String no formato css</returns>
        public string GetBottomStringValue()
        {
            return Bottom.HasValue ? String.Format("{0}{1}", Bottom.Value, Unit.ToString().ToLower()) : "";
        }

        /// <summary>
        /// Retorna uma string no formato valor + unidade que represente a coordenada esquerda.
        /// </summary>
        /// <returns>String no formato css</returns>
        public string GetLeftStringValue()
        {
            return Left.HasValue ? String.Format("{0}{1}", Left.Value, Unit.ToString().ToLower()) : "";
        }

        /// <summary>
        /// Retorna uma string no formato valor + unidade que represente a coordenada direita.
        /// </summary>
        /// <returns>String no formato css</returns>
        public string GetRightStringValue()
        {
            return Right.HasValue ? String.Format("{0}{1}", Right.Value, Unit.ToString().ToLower()) : "";
        }

        /// <summary>
        /// Retorna 'true' caso alguma coordenada possuir um valor diferente de null./>
        /// </summary>
        internal bool IsSetted()
        {
            return Top.HasValue || Bottom.HasValue || Left.HasValue || Right.HasValue;
        }

        /// <summary>
        /// Retorna 'true' caso TODAS coordenadas possuirem um valor diferente de null./>
        /// </summary>
        internal bool AllValuesIsSetted()
        {
            return Top.HasValue && Bottom.HasValue && Left.HasValue && Right.HasValue;
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public String GetAllValuesString()
        {
            return String.Format("{1}{0} {2}{0} {3}{0} {4}{0}",
                Unit.ToString().ToLower(),
                Top.GetValueOrDefault(0),
                Right.GetValueOrDefault(0),
                Bottom.GetValueOrDefault(0),
                Left.GetValueOrDefault(0));
        }

        public override int GetHashCode()
        {
            return GetAllValuesString().GetHashCode();
        }

        public override string ToString()
        {
            return GetAllValuesString();
        }

        /// <summary>
        /// Evento disparado se alguma coordenada ou a unidade css forem alteradas.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

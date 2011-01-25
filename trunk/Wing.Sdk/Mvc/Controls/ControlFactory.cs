using System;
using System.Collections.Generic;
using System.Linq;


namespace Wing.Mvc.Controls
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class ControlFactory
    {
        private static List<IControlBuildExtension> _extenders = new List<IControlBuildExtension>();
        private static Dictionary<Type, List<HtmlObjectDelegate>> _controlsExtensions = new Dictionary<Type, List<HtmlObjectDelegate>>();
        private static Dictionary<Type, List<HtmlObjectDelegate>> _extensionDelegatesMap = new Dictionary<Type, List<HtmlObjectDelegate>>();

        public static void AddNewExtender<TType>() where TType : IControlBuildExtension
        {
            _extenders.Add((IControlBuildExtension)Activator.CreateInstance(typeof(TType)));
        }

        public static void AddExtender(IControlBuildExtension extender)
        {
            _extenders.Add(extender);
        }

        public static void ExtendControl<TControlType>(TypedControlDelegate<TControlType> extensionDelegate) where TControlType : HtmlObject
        {
            Assert.NullArgument(extensionDelegate, "extensionDelegate");
            var controlType = typeof(TControlType);
            List<HtmlObjectDelegate> delegates = null;
            if (!_controlsExtensions.TryGetValue(controlType, out delegates))
                delegates = _controlsExtensions[controlType] = new List<HtmlObjectDelegate>();
            delegates.Add(new HtmlObjectDelegate((_control) =>
            {
                try
                {
                    extensionDelegate.Invoke((TControlType)_control);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error on control extension delegate", ex);
                }
            }));
            _extensionDelegatesMap.Clear();
            CheckControlInheritanceTree(controlType);
        }

        private static void CheckControlInheritanceTree(Type controlType)
        {
            if (_extensionDelegatesMap.ContainsKey(controlType))
                return;

            var inheritanceTree = new List<Type>();
            var queue = new Queue<Type>();
            queue.Enqueue(controlType);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                inheritanceTree.Add(item);
                if (typeof(HtmlObject).IsAssignableFrom(item.BaseType))
                    queue.Enqueue(item.BaseType);
            }
            var extensions = new List<HtmlObjectDelegate>();
            foreach (var type in inheritanceTree.Distinct())
            {
                List<HtmlObjectDelegate> typeDelegates = null;
                if (_controlsExtensions.TryGetValue(type, out typeDelegates))
                    extensions.AddRange(typeDelegates);
            }
            extensions.Reverse();
            _extensionDelegatesMap[controlType] = extensions;
        }

        internal static void ExtendControl(HtmlObject control, ExtensionStage stage)
        {
            foreach (IControlBuildExtension extender in _extenders)
                extender.Extend(control, stage);
        }

        internal static void PerformExtensionDelegates(HtmlObject control)
        {
            if (!_extensionDelegatesMap.ContainsKey(control.GetType()))
                return;
            var delegates = _extensionDelegatesMap[control.GetType()];
            foreach (var delegateItem in delegates)
                delegateItem.Invoke(control);
        }

    }
}
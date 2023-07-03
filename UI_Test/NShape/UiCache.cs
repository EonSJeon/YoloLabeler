using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace UI_Test.NShape
{
    internal class UiCache
    {
        public static Dictionary<string, FrameworkElement> cache = new Dictionary<string, FrameworkElement>();

        public static void ScanAll(DependencyObject target)
        {
            var fe2 = target as FrameworkElement;
            if (fe2 != null && fe2.Name.Length > 0)
            {
              //  Debug.WriteLine("*** == NAME {0} ", fe2.Name);
                if (!cache.ContainsKey(fe2.Name))
                {
                    cache[fe2.Name] = fe2;
                }
            }
            var childeren = LogicalTreeHelper.GetChildren(target);
            foreach (var childerenElement in childeren)
            {
                var fe = childerenElement as FrameworkElement;
                if (fe != null)
                {
                    ScanAll(fe);
                }
            }
        }
        public static FrameworkElement Find(string name)
        {
            if (cache.ContainsKey(name))
            {
                return cache[name];
            }
          //  Debug.Fail("UICache.NotFound Name=" + name);
            return null;

        }
        public DependencyObject FindElement_xx(string findName)
        {

            // 이동 및 , 캐쉬적용.
            //DependencyObject parent = VisualTreeHelper.GetParent(TableImage);
            //while (parent != null)
            //{
            //    Debug.WriteLine("== Search0 Type= {0}", parent.GetType());
            //    parent = VisualTreeHelper.GetParent(parent);
            //}
            //var parents = parent.GetSelfAndAncestors();
            //foreach (DependencyObject item in parents)
            //{
            //    Debug.WriteLine("== Search0 Type= {0}",item.GetType());
            //    if (typeof(FrameworkElement).IsInstanceOfType(item))
            //    {
            //        FrameworkElement _target = (FrameworkElement)item;

            //        if (_target.Name == findName)
            //        {
            //            return item;
            //        }
            //    }
            //}
            //foreach (DependencyObject pi in parents)
            //{
            //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(pi); i++)
            //    {
            //        DependencyObject item2 = VisualTreeHelper.GetChild(pi, i);
            //        if (typeof(FrameworkElement).IsInstanceOfType(item2))
            //        {
            //            FrameworkElement _target = (FrameworkElement)item2;
            //            Debug.WriteLine("===   Search2 Type= {0}", _target.GetType());
            //            if (_target.Name == findName)
            //            {
            //                return item2;

            //            }
            //        }
            //    }
            //}
            return null;
        }
    }
}

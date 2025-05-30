using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NoteBook.Extension
{
    public class PassWordBoxExtend:DependencyObject
    {
        public static void SetPassword(DependencyObject element, string value)
        {
            element.SetValue(PasswordProperty, value);
        }
        public static string GetPassword(DependencyObject element)
        {
            return (string)element.GetValue(PasswordProperty);
        }
        public static readonly System.Windows.DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PassWordBoxExtend), new PropertyMetadata(string.Empty, OnPasswordPropertyChange));

        private static void OnPasswordPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox && passwordBox.Password != (string)e.NewValue)
            {
                passwordBox.Password = (string)e.NewValue;
            }
        }
    }
    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                PassWordBoxExtend.SetPassword(passwordBox, passwordBox.Password);
            }

        }
        override protected void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PasswordChanged += PasswordChanged;
            PassWordBoxExtend.SetPassword(AssociatedObject, AssociatedObject.Password);
        }

        override protected void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PasswordChanged -= PasswordChanged;
        }

    }
}

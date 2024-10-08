﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text.RegularExpressions;
using CommunityToolkit.Common;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#endif

namespace Project2FA.Extensions
{
    /// <inheritdoc cref="TextBoxExtensions"/>
    public static partial class TextBoxExtensions
    {
        private static void TextBoxRegexPropertyOnChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            textBox.LostFocus -= TextBox_LostFocus;
            textBox.BeforeTextChanging -= TextBox_BeforeTextChanging;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.BeforeTextChanging += TextBox_BeforeTextChanging;
            textBox.GettingFocus -= TextBox_GettingFocus;
            textBox.GettingFocus += TextBox_GettingFocus;
        }

        private static void TextBox_BeforeTextChanging(TextBox textBox, TextBoxBeforeTextChangingEventArgs args)
        {
            var validationMode = (ValidationMode)textBox.GetValue(ValidationModeProperty);
            var validationType = (ValidationType)textBox.GetValue(ValidationTypeProperty);
            var (valid, successful) = ValidateTextBox(textBox, args.NewText, validationMode != ValidationMode.Normal);
            if (successful &&
                !valid &&
                validationMode == ValidationMode.Dynamic &&
                validationType != ValidationType.Email &&
                validationType != ValidationType.PhoneNumber &&
                args.NewText != string.Empty)
            {
                textBox.Tag = "ValidationError";
                args.Cancel = true;
            }
            else
            {
                textBox.Tag = string.Empty; //valid input
                // workaround for the valid charcaters after initial invalid input
                // where the next valid input was set on wrong position
                if (valid && textBox.Text.Length == 0 && args.NewText.Length == 1)
                {
                    textBox.SelectionStart = 1;
                }
            }
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            ValidateTextBox(textBox, textBox.Text);
        }

        private static void TextBox_GettingFocus(UIElement sender, GettingFocusEventArgs args)
        {
            var textBox = (TextBox)sender;
            textBox.SelectionStart = textBox.Text.Length;
        }

        private static (bool valid, bool successful) ValidateTextBox(TextBox textBox, string newText, bool force = true)
        {
            var validationType = (ValidationType)textBox.GetValue(ValidationTypeProperty);
            string regex;
            bool regexMatch;

            switch (validationType)
            {
                default:
                    regex = textBox.GetValue(RegexProperty) as string;
                    if (string.IsNullOrWhiteSpace(regex))
                    {
                        Debug.WriteLine("Regex property can't be null or empty when custom mode is selected");
                        return (false, false);
                    }

                    regexMatch = Regex.IsMatch(newText, regex);
                    break;
                case ValidationType.Decimal:
                    regexMatch = newText.IsDecimal();
                    break;
                case ValidationType.Email:
                    regexMatch = newText.IsEmail();
                    break;
                case ValidationType.Number:
                    regexMatch = newText.IsNumeric();
                    break;
                case ValidationType.PhoneNumber:
                    regexMatch = newText.IsPhoneNumber();
                    break;
                case ValidationType.Characters:
                    regexMatch = newText.IsCharacterString();
                    break;
            }

            var isValid = (bool)textBox.GetValue(IsValidProperty);
            if (regexMatch == false && force && newText != string.Empty)
            {
                var validationModel = (ValidationMode)textBox.GetValue(ValidationModeProperty);
                if (validationModel == ValidationMode.Forced)
                {
                    if (textBox.Text == newText)
                    {
                        // occurs when focus is lost
                        textBox.Text = string.Empty;
                    }
                    else
                    {
                        // only set IsValidProperty to false, when the property is true
                        if (isValid)
                        {
                            textBox.SetValue(IsValidProperty, regexMatch);
                        }
                    }
                }
            }
            else
            {
                // only set IsValidProperty when the property is not equal to regexMatch
                if (isValid != regexMatch)
                {
                    textBox.SetValue(IsValidProperty, regexMatch);
                }
            }

            return (regexMatch, true);
        }
    }
}

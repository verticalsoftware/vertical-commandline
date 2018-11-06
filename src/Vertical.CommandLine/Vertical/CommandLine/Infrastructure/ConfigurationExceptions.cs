// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Vertical.CommandLine.Configuration;
using Vertical.CommandLine.Parsing;
using static Vertical.CommandLine.Infrastructure.Formatting;

namespace Vertical.CommandLine.Infrastructure
{
    /// <summary>
    /// Factory for configuration related exceptions.
    /// </summary>
    internal static class ConfigurationExceptions
    {
        // No default converter.
        internal static Exception NoDefaultConverter<TValue>()
        {
            return new ConfigurationException(
                $"No default converter available for type {FriendlyName(typeof(TValue))}."
            );
        }

        // No property mapper defined.
        internal static Exception NoPropertyMapper()
        {
            return new ConfigurationException("No property mapper defined.");
        }
        
        // Not a property expression
        internal static Exception NotAPropertyExpression<TOptions, TValue>(Expression<Func<TOptions, TValue>> expression)
        {
            return new ConfigurationException($"{Quote(expression)} is not a property expression." +
                                              "and cannot be used in a property mapper.");
        }

        // Read-only property
        internal static Exception NotWriteableProperty(PropertyInfo propertyInfo)
        {
            return new ConfigurationException($"Property {FriendlyName(propertyInfo.DeclaringType)}.{propertyInfo.Name}" +
                                              "is read-only and cannot be used in a property mapper. Either use a different " +
                                              "property or add a set accessor.");
        }

        // Template token in use.
        internal static Exception TemplateTokenInUse(Token token)
        {
            return new ConfigurationException($"{Quote(token.DistinguishedForm)} is already in use by another option or switch.");
        }

        // Error in argument configuration
        internal static Exception InvalidParserConfiguration(string context, Exception innerException)
        {
            return new ConfigurationException($"{context} - {innerException.Message}");
        }
        
        // Error in configuration
        internal static Exception InvalidCommandConfiguration(string context, Exception innerException)
        {
            return new ConfigurationException($"Invalid configuration for {context}: {innerException.Message}");
        }

        // No default options constructor
        internal static Exception NoDefaultOptionsConstructor<TOptions>()
        {
            return new ConfigurationException($"No accessible default constructor found for {FriendlyName(typeof(TOptions))}.");
        }

        // No client handler defined
        internal static Exception NoClientHandlerDefined(string applicationName)
        {
            return new ConfigurationException(
                $"No client handler defined for {applicationName}." +
                Environment.NewLine +
                "Correct this by calling .OnExecute() or .OnExecuteAsync() in the configuration and providing the appropriate delegate.");
        }

        // No help content provider
        internal static Exception NoHelpContentProviderDefined()
        {
            return new ConfigurationException("Help was invoked by a matched option, but no help content provider configured.");
        }

        internal static Exception InvalidOptionOrSwitchTemplate(string template)
        {
            return new ConfigurationException(
                $"Invalid template \"{template}\" - option/switch must contain one or more short (single dash) " +
                "or long form (double-dash) identifiers separated by a pipe. E.g. (-h, --help, -h|--help, etc.)");
        }

        internal static Exception InvalidCommandTemplate(string template)
        {
            return new ConfigurationException(
                $"Invalid template \"{template}\" - command can only contain plain word values " +
                "that are not prefixed with a dash.");
        }

        internal static Exception NullReferenceInMapping<TOptions>(Exception innerException)
        {
            return new ConfigurationException(
                "Object reference not set to an instance of an object. " +
                Environment.NewLine +
                "Since mapping is performed using a delegate, " +
                $"make sure that the target member is properly initialized in the {FriendlyName(typeof(TOptions))} class",
                innerException);
        }

        internal static Exception ErrorInPropertyMapping<TOptions>(string property, Exception innerException)
        {
            return new ConfigurationException(
                $"An error occurred while mapping value to property '{property}' in options class {FriendlyName(typeof(TOptions))}." +
                Environment.NewLine +
                "Verify the application logic found in your delegate code or interface implementation.",
                innerException);
        }

        internal static Exception ErrorInDelegateMapping(Exception ex)
        {
            return new ConfigurationException("An error occurred while mapping the argument or switch that occurred in the client-defined delegate code.", ex);
        }

        internal static Exception ErrorInCollectionMapping<TOptions>(string collectionType, string property, Exception innerException)
        {
            return new ConfigurationException(
                $"An error occurred while mapping value to {collectionType} property '{property}' in options class {FriendlyName(typeof(TOptions))}." +
                Environment.NewLine +
                "Verify the application logic found in your delegate code or interface implementation.", innerException);
        }

        internal static Exception NullReferenceInCollectionMapping<TOptions>(string collectionType, string property, Exception innerException)
        {
            return new ConfigurationException(
                "Object reference not set to an instance of an object. "+
                Environment.NewLine +
                $"It could be that the {collectionType} property named \"{property}\" " +
                $"in the {FriendlyName(typeof(TOptions))} class is not initialized.",
                innerException);
        }

        internal static Exception HandlerSynchAsyncMismatch(Type expectedReturn)
        {
            var handlerType = typeof(Task).IsAssignableFrom(expectedReturn) ? "asynchronous" : "synchronous";

            return new ConfigurationException(
                $"Selected client handler should be {handlerType} but was not." +
                Environment.NewLine +
                "The handler types must be consistent across commands and the root configuration.");
        }

        internal static Exception MappingFailed(string context, Exception exception)
        {
            return new ConfigurationException($"{context}: mapping failed - {exception.Message}", exception);
        }

        internal static Exception NoHelpOptionDefined()
        {
            return new ConfigurationException("Help was programmatically invoked but no help option has been defined.");
        }

        internal static Exception HelpProviderReturnedNull(Type providerType)
        {
            return new ConfigurationException($"Content provider {FriendlyName(providerType)} returned null string collection.");
        }

        internal static Exception HelpProviderFailed(Type providerType, Exception innerException)
        {
            return new ConfigurationException($"Content provider {FriendlyName(providerType)} threw an exception.",
                innerException);
        }

        internal static Exception OptionsProviderReturnedNull(Type optionsType)
        {
            return new ConfigurationException($"Options provider threw an exception when asking for a {FriendlyName(optionsType)} instance.");
        }

        internal static Exception OptionsProviderFailed(Type optionsType, Exception innerException = null)
        {
            return new ConfigurationException(
                $"Options provider failed when asking for a {FriendlyName(optionsType)} instance.",
                innerException
            );
        }
    }
}
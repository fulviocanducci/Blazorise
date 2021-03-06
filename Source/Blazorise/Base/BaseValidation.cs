﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseValidation : ComponentBase
    {
        #region Members

        /// <summary>
        /// Holds the last input value.
        /// </summary>
        private object value;

        /// <summary>
        /// Raises an event that the validation has started.
        /// </summary>
        public event ValidatingEventHandler Validating;

        /// <summary>
        /// Raises an event after the validation has passes successfully.
        /// </summary>
        public event ValidationSucceededEventHandler ValidationSucceeded;

        /// <summary>
        /// Raises an event after the validation has failed.
        /// </summary>
        public event ValidationFailedEventHandler ValidationFailed;

        internal event ValidatedEventHandler Validated;

        #endregion

        #region Methods

        protected override void OnInit()
        {
            if ( ParentValidations != null )
            {
                ParentValidations.ValidatingAll += OnValidatingAll;
                ParentValidations.ClearingAll += OnClearingAll;
            }

            base.OnInit();
        }

        public void Dispose()
        {
            if ( ParentValidations != null )
            {
                ParentValidations.ValidatingAll -= OnValidatingAll;
                ParentValidations.ClearingAll -= OnClearingAll;
            }
        }

        internal void InitInputValue( object value )
        {
            // save the input value
            this.value = value;

            if ( Mode == ValidationMode.Auto )
                Validate();
        }

        bool AreEqual( Array array1, Array array2 )
        {
            if ( array1 == null && array2 == null )
                return true;

            if ( array1 != null && array2 != null
                && array1.Length == array2.Length )
            {
                for ( int i = 0; i < array1.Length; ++i )
                {
                    if ( array1.GetValue( i ) != array2.GetValue( i ) )
                        return false;
                }

                return true;
            }

            return false;
        }

        internal void UpdateInputValue( object value )
        {
            if ( value is Array )
            {
                if ( !AreEqual( this.value as Array, value as Array ) )
                {
                    this.value = value;

                    if ( Mode == ValidationMode.Auto )
                        Validate();
                }
            }
            else
            {
                if ( this.value != value )
                {
                    this.value = value;

                    if ( Mode == ValidationMode.Auto )
                        Validate();
                }
            }
        }

        private void OnValidatingAll( ValidatingAllEventArgs e )
        {
            e.Cancel = Validate() == ValidationStatus.Error;
        }

        private void OnClearingAll()
        {
            Clear();
        }

        /// <summary>
        /// Runs the validation process.
        /// </summary>
        public ValidationStatus Validate()
        {
            var handler = Validator;

            //Status = ValidationStatus.None;

            if ( handler != null )
            {
                Validating?.Invoke();

                var args = new ValidatorEventArgs( value );

                handler( args );

                if ( Status != args.Status )
                {
                    Status = args.Status;

                    if ( args.Status == ValidationStatus.Success )
                        ValidationSucceeded?.Invoke( new ValidationSucceededEventArgs() );
                    else if ( args.Status == ValidationStatus.Error )
                        ValidationFailed?.Invoke( new ValidationFailedEventArgs( args.ErrorText ) );

                    Validated?.Invoke( new ValidatedEventArgs( Status, args.ErrorText ) );

                    StateHasChanged();
                }
            }

            // force the reload of all child components
            //StateHasChanged();

            return Status;
        }

        /// <summary>
        /// Clears the validation status.
        /// </summary>
        public void Clear()
        {
            Status = ValidationStatus.None;
            StateHasChanged();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the validation mode.
        /// </summary>
        private ValidationMode Mode => ParentValidations?.Mode ?? ValidationMode.Auto;

        /// <summary>
        /// Gets or sets the current validation status.
        /// </summary>
        [Parameter] protected internal ValidationStatus Status { get; set; }

        /// <summary>
        /// Validates the input value after it has being changed.
        /// </summary>
        [Parameter] protected Action<ValidatorEventArgs> Validator { get; set; }

        [CascadingParameter] protected BaseValidations ParentValidations { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}

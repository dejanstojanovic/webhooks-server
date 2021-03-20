using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class GuidNotEmptyAttribute : ValidationAttribute
    {
        public GuidNotEmptyAttribute():base()
        {
            this.ErrorMessage = "Only valid not empty GUID values allowed";
        }
        public override bool IsValid(object value)
        {
            try
            {
                var guidString = value.ToString();
                var guid = new Guid(guidString);
                if (guid == Guid.Empty)
                    return false;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}

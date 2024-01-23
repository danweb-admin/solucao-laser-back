using System;
namespace Solucao.Application.Exceptions.Calendar
{
	public class CalendarNoValueException : Exception
    {
		public CalendarNoValueException()
		{
		}

        public CalendarNoValueException(string message)
            : base(message)
        {

        }
    }
}


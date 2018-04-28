using System.ComponentModel;

namespace NetworkModel
{
	public enum NetworkType
	{
		/// <summary>
		/// Автомат Мили
		/// </summary>
		[Description("Мили")]
		Mealy,

		/// <summary>
		/// Автомат Мура
		/// </summary>
		[Description("Мура")]
		Moore
	}
}
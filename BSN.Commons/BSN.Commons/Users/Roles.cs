﻿using System.ComponentModel.DataAnnotations;

namespace Commons.Users
{
	public enum Roles
	{
		[Display(Name = "مهمان")]
		Guest,
		[Display(Name = "مدیر سیستم")]
		Admin,
		[Display(Name = "دکتر")]
		Doctor,
		[Display(Name = "منشی")]
		Secretary
	}
}

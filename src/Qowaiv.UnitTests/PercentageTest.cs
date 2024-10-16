﻿using NUnit.Framework;
using Qowaiv.UnitTests.Json;
using Qowaiv.UnitTests.TestTools;
using Qowaiv.UnitTests.TestTools.Formatting;
using Qowaiv.UnitTests.TestTools.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Qowaiv.UnitTests
{
	[TestFixture]
	public class PercentageTest
	{
		/// <summary>The test instance for most tests.</summary>
		public static readonly Percentage TestStruct = 0.1751m;

		#region Percentage const tests

		/// <summary>Percentage.Zero should be equal to the default of Percentage.</summary>
		[Test]
		public void Zero_None_EqualsDefault()
		{
			Assert.AreEqual(default(Percentage), Percentage.Zero);
		}

		[Test]
		public void One_None_0Dot01()
		{
			Assert.AreEqual((Percentage)0.01m, Percentage.One);
		}

		[Test]
		public void Hundred_None_1()
		{
			Assert.AreEqual((Percentage)1m, Percentage.Hundred);
		}

		#endregion

		#region TryParse tests

		/// <summary>TryParse null should be valid.</summary>
		[Test]
		public void TyrParse_Null_IsInvalid()
		{
			Percentage val;

			string str = null;

			Assert.IsFalse(Percentage.TryParse(str, out val), "Valid");
			Assert.AreEqual(Percentage.Zero, val, "Value");
		}

		/// <summary>TryParse string.Empty should be valid.</summary>
		[Test]
		public void TyrParse_StringEmpty_IsInvalid()
		{
			Percentage val;

			string str = string.Empty;

			Assert.IsFalse(Percentage.TryParse(str, out val), "Valid");
			Assert.AreEqual(Percentage.Zero, val, "Value");
		}

		/// <summary>TryParse with specified string value should be valid.</summary>
		[Test]
		public void TyrParse_StringValue_IsValid()
		{
			using (new CultureInfoScope("nl-NL"))
			{
				Percentage val;

				string str = "17,51%";

				Assert.IsTrue(Percentage.TryParse(str, out val), "Valid");
				Assert.AreEqual(str, val.ToString(), "Value");
			}
		}

		/// <summary>TryParse with specified string value should be invalid.</summary>
		[Test]
		public void TyrParse_StringValue_IsNotValid()
		{
			Percentage val;

			string str = "string";

			Assert.IsFalse(Percentage.TryParse(str, out val), "Valid");
			Assert.AreEqual(Percentage.Zero, val, "Value");
		}

		[Test]
		public void Parse_InvalidInput_ThrowsFormatException()
		{
			using (new CultureInfoScope("en-GB"))
			{
				Assert.Catch<FormatException>
				(() =>
				{
					Percentage.Parse("InvalidInput");
				},
				"Not a valid percentage");
			}
		}

		[Test]
		public void ParseFrench_Percentage17Comma51_AreEqual()
		{
			using (new CultureInfoScope("fr-FR"))
			{
				var dec = 0.1751m;
				var str = dec.ToString("%0.000");
				//var actDecimal = Decimal.Parse("17,51%");
				//Assert.AreEqual((Decimal)TestStruct, actDecimal, "Decimal");


				var actPercent = Percentage.Parse("%17,51");
				Assert.AreEqual(TestStruct, actPercent, "Percentage");
			}
		}

		[Test]
		public void TryParse_PercentageMarkInvalidPosition_AreEqual()
		{
			Percentage exp;
			Percentage act = Percentage.Zero;

			Assert.IsFalse(Percentage.TryParse("2%2", out exp));
			Assert.AreEqual(exp, act);
		}
		[Test]
		public void TryParse_PerMilleMarkInvalidPosition_AreEqual()
		{
			Percentage exp;
			Percentage act = Percentage.Zero;

			Assert.IsFalse(Percentage.TryParse("2‰2", out exp));
			Assert.AreEqual(exp, act);
		}
		[Test]
		public void TryParse_PerTenThousendMarkInvalidPosition_AreEqual()
		{
			Percentage exp;
			Percentage act = Percentage.Zero;

			Assert.IsFalse(Percentage.TryParse("2‱2", out exp));
			Assert.AreEqual(exp, act);
		}

		#endregion

		#region Create tests

		[Test]
		public void Create_DecimalValue_AreEqual()
		{
			var exp = TestStruct;
			var act = Percentage.Create(0.1751m);
			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Create_DoubleValue_AreEqual()
		{
			var exp = TestStruct;
			var act = Percentage.Create(0.1751);
			Assert.AreEqual(exp, act);
		}

		#endregion

		#region (XML) (De)serialization tests

		[Test]
		public void Constructor_SerializationInfoIsNull_ThrowsArgumentNullException()
		{
			ExceptionAssert.CatchArgumentNullException
			(() =>
			{
				SerializationTest.DeserializeUsingConstructor<Percentage>(null, default(StreamingContext));
			},
			"info");
		}

		[Test]
		public void Constructor_InvalidSerializationInfo_ThrowsSerializationException()
		{
			Assert.Catch<SerializationException>
			(() =>
			{
				var info = new SerializationInfo(typeof(Percentage), new System.Runtime.Serialization.FormatterConverter());
				SerializationTest.DeserializeUsingConstructor<Percentage>(info, default(StreamingContext));
			});
		}

		[Test]
		public void GetObjectData_Null_ThrowsArgumentNullException()
		{
			ExceptionAssert.CatchArgumentNullException
			(() =>
			{
				ISerializable obj = TestStruct;
				obj.GetObjectData(null, default(StreamingContext));
			},
			"info");
		}

		[Test]
		public void GetObjectData_SerializationInfo_AreEqual()
		{
			ISerializable obj = TestStruct;
			var info = new SerializationInfo(typeof(Percentage), new System.Runtime.Serialization.FormatterConverter());
			obj.GetObjectData(info, default(StreamingContext));

			Assert.AreEqual(0.1751m, info.GetDecimal("Value"));
		}

		[Test]
		public void SerializeDeserialize_TestStruct_AreEqual()
		{
			var input = PercentageTest.TestStruct;
			var exp = PercentageTest.TestStruct;
			var act = SerializationTest.SerializeDeserialize(input);
			Assert.AreEqual(exp, act);
		}
		[Test]
		public void DataContractSerializeDeserialize_TestStruct_AreEqual()
		{
			var input = PercentageTest.TestStruct;
			var exp = PercentageTest.TestStruct;
			var act = SerializationTest.DataContractSerializeDeserialize(input);
			Assert.AreEqual(exp, act);
		}
		[Test]
		public void XmlSerializeDeserialize_TestStruct_AreEqual()
		{
			var input = PercentageTest.TestStruct;
			var exp = PercentageTest.TestStruct;
			var act = SerializationTest.XmlSerializeDeserialize(input);
			Assert.AreEqual(exp, act);
		}

		[Test]
		public void SerializeDeserialize_PercentageSerializeObject_AreEqual()
		{
			var input = new PercentageSerializeObject()
			{
				Id = 17,
				Obj = PercentageTest.TestStruct,
				Date = new DateTime(1970, 02, 14),
			};
			var exp = new PercentageSerializeObject()
			{
				Id = 17,
				Obj = PercentageTest.TestStruct,
				Date = new DateTime(1970, 02, 14),
			};
			var act = SerializationTest.SerializeDeserialize(input);
			Assert.AreEqual(exp.Id, act.Id, "Id");
			Assert.AreEqual(exp.Obj, act.Obj, "Obj");
			DateTimeAssert.AreEqual(exp.Date, act.Date, "Date");;
		}
		[Test]
		public void XmlSerializeDeserialize_PercentageSerializeObject_AreEqual()
		{
			var input = new PercentageSerializeObject()
			{
				Id = 17,
				Obj = PercentageTest.TestStruct,
				Date = new DateTime(1970, 02, 14),
			};
			var exp = new PercentageSerializeObject()
			{
				Id = 17,
				Obj = PercentageTest.TestStruct,
				Date = new DateTime(1970, 02, 14),
			};
			var act = SerializationTest.XmlSerializeDeserialize(input);
			Assert.AreEqual(exp.Id, act.Id, "Id");
			Assert.AreEqual(exp.Obj, act.Obj, "Obj");
			DateTimeAssert.AreEqual(exp.Date, act.Date, "Date");;
		}
		[Test]
		public void DataContractSerializeDeserialize_PercentageSerializeObject_AreEqual()
		{
			var input = new PercentageSerializeObject()
			{
				Id = 17,
				Obj = PercentageTest.TestStruct,
				Date = new DateTime(1970, 02, 14),
			};
			var exp = new PercentageSerializeObject()
			{
				Id = 17,
				Obj = PercentageTest.TestStruct,
				Date = new DateTime(1970, 02, 14),
			};
			var act = SerializationTest.DataContractSerializeDeserialize(input);
			Assert.AreEqual(exp.Id, act.Id, "Id");
			Assert.AreEqual(exp.Obj, act.Obj, "Obj");
			DateTimeAssert.AreEqual(exp.Date, act.Date, "Date");;
		}

		[Test]
		public void SerializeDeserialize_Empty_AreEqual()
		{
			var input = new PercentageSerializeObject()
			{
				Id = 17,
				Obj = PercentageTest.TestStruct,
				Date = new DateTime(1970, 02, 14),
			};
			var exp = new PercentageSerializeObject()
			{
				Id = 17,
				Obj = PercentageTest.TestStruct,
				Date = new DateTime(1970, 02, 14),
			};
			var act = SerializationTest.SerializeDeserialize(input);
			Assert.AreEqual(exp.Id, act.Id, "Id");
			Assert.AreEqual(exp.Obj, act.Obj, "Obj");
			DateTimeAssert.AreEqual(exp.Date, act.Date, "Date");;
		}

		[Test]
		public void GetSchema_None_IsNull()
		{
			IXmlSerializable obj = TestStruct;
			Assert.IsNull(obj.GetSchema());
		}

		#endregion

		#region JSON (De)serialization tests

		[Test]
		public void FromJson_Null_AssertNotSupportedException()
		{
			Assert.Catch<NotSupportedException>(() =>
			{
				JsonTester.Read<Percentage>();
			},
			"JSON deserialization from null is not supported.");
		}

		[Test]
		public void FromJson_InvalidStringValue_AssertFormatException()
		{
			Assert.Catch<FormatException>(() =>
			{
				JsonTester.Read<Percentage>("InvalidStringValue");
			},
			"Not a valid percentage");
		}
		[Test]
		public void FromJson_StringValue_AreEqual()
		{
			var act = JsonTester.Read<Percentage>(TestStruct.ToString(CultureInfo.InvariantCulture));
			var exp = TestStruct;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void FromJson_Int64Value_AssertNotSupportedException()
		{
			Assert.Catch<NotSupportedException>(() =>
			{
				JsonTester.Read<Percentage>(123456L);
			},
			"JSON deserialization from an integer is not supported.");
		}

		[Test]
		public void FromJson_DoubleValue_AreEqual()
		{
			var act = JsonTester.Read<Percentage>((Double)TestStruct);
			var exp = TestStruct;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void FromJson_DateTimeValue_AssertNotSupportedException()
		{
			Assert.Catch<NotSupportedException>(() =>
			{
				JsonTester.Read<Percentage>(new DateTime(1972, 02, 14));
			},
			"JSON deserialization from a date is not supported.");
		}

		[Test]
		public void ToJson_DefaultValue_AreEqual()
		{
			object act = JsonTester.Write(default(Percentage));
			object exp = "0%";

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void ToJson_TestStruct_AreEqual()
		{
			var act = JsonTester.Write(TestStruct);
			var exp = TestStruct.ToString(CultureInfo.InvariantCulture);

			Assert.AreEqual(exp, act);
		}

		#endregion

		#region IFormattable / Tostring tests

		[Test]
		public void DebuggerDisplay_DebugToString_HasAttribute()
		{
			DebuggerDisplayAssert.HasAttribute(typeof(Percentage));
		}

		[Test]
		public void DebuggerDisplay_DefaultValue_String()
		{
			DebuggerDisplayAssert.HasResult("17.51%", TestStruct);
		}

		[Test]
		public void ToString_CustomFormatter_SupportsCustomFormatting()
		{
			using (CultureInfoScope.NewInvariant())
			{
				var act = TestStruct.ToString("Unit Test Format", new UnitTestFormatProvider());
				var exp = "Unit Test Formatter, value: '17.51%', format: 'Unit Test Format'";

				Assert.AreEqual(exp, act);
			}
		}

		[Test]
		public void ToString_IFormatProviderNull_FormattedString()
		{
			using (new CultureInfoScope("nl-BE"))
			{
				var act = TestStruct.ToString((IFormatProvider)null);
				var exp = "17,51%";

				Assert.AreEqual(exp, act);
			}
		}

		[Test]
		public void ToString_Zero_0()
		{
			using (new CultureInfoScope("en-GB"))
			{
				var act = Percentage.Zero.ToString();
				var exp = "0%";
				Assert.AreEqual(exp, act);
			}
		}

		[Test]
		public void ToString_ZeroNullFormat_0()
		{
			using (new CultureInfoScope("en-GB"))
			{
				var act = Percentage.Zero.ToString((String)null);
				var exp = "0";
				Assert.AreEqual(exp, act);
			}
		}

		[Test]
		public void ToPercentageString_TestStruct_0()
		{
			using (new CultureInfoScope("en-GB"))
			{
				var act = TestStruct.ToString();
				var exp = "17.51%";
				Assert.AreEqual(exp, act);
			}
		}
		[Test]
		public void ToPerMilleString_TestStruct_0()
		{
			using (new CultureInfoScope("en-GB"))
			{
				var act = TestStruct.ToPerMilleString();
				var exp = "175.1‰";
				Assert.AreEqual(exp, act);
			}
		}
		[Test]
		public void ToPerTenThousendMarkString_TestStruct_0()
		{
			using (new CultureInfoScope("en-GB"))
			{
				var act = TestStruct.ToPerTenThousendMarkString();
				var exp = "1751‱";
				Assert.AreEqual(exp, act);
			}
		}

		[Test]
		public void ToString_ValueDutchBelgium_AreEqual()
		{
			using (new CultureInfoScope("nl-BE"))
			{
				var act = Percentage.Parse("1600,1").ToString();
				var exp = "1600,1%";
				Assert.AreEqual(exp, act);
			}
		}
		[Test]
		public void ToString_ValueDutchNetherlands_AreEqual()
		{
			using (new CultureInfoScope("nl-NL"))
			{
				var act = Percentage.Parse("1600,1").ToString("");
				var exp = "1600,1";
				Assert.AreEqual(exp, act);
			}
		}

		[Test]
		public void ToString_ValueEnglishGreatBritain_AreEqual()
		{
			using (new CultureInfoScope("en-GB"))
			{
				var act = Percentage.Parse("1600.1").ToString();
				var exp = "1600.1%";
				Assert.AreEqual(exp, act);
			}
		}

		[Test]
		public void ToString_FormatValueDutchBelgium_AreEqual()
		{
			using (new CultureInfoScope("nl-BE"))
			{
				var act = Percentage.Parse("800").ToString("0000");
				var exp = "0800";
				Assert.AreEqual(exp, act);
			}
		}

		[Test]
		public void ToString_FormatValueEnglishGreatBritain_AreEqual()
		{
			using (new CultureInfoScope("en-GB"))
			{
				var act = Percentage.Parse("800").ToString("0000");
				var exp = "0800";
				Assert.AreEqual(exp, act);
			}
		}

		[Test]
		public void ToString_FormatValueSpanishEcuador_AreEqual()
		{
			var act = Percentage.Parse("1700").ToString("00000.0", new CultureInfo("es-EC"));
			var exp = "01700,0";
			Assert.AreEqual(exp, act);
		}

		[Test]
		public void ToString_PercentageEnGB_FormattedString()
		{
			using (new CultureInfoScope("en-GB"))
			{
				Assert.AreEqual("31.415%", ((Percentage).31415).ToString());
			}
		}

		[Test]
		public void ToString_PercentageNlBE_FormattedString()
		{
			using (new CultureInfoScope("nl-BE"))
			{
				Assert.AreEqual("31,415%", ((Percentage).31415).ToString());
			}
		}

		[Test]
		public void ToString_PercentageFrFR_FormattedString()
		{
			using (new CultureInfoScope("fr-FR"))
			{
				Assert.AreEqual("%31,415", ((Percentage).31415).ToString());
			}
		}
		[Test]
		public void ToString_PercentageFaIR_FormattedString()
		{
			using (new CultureInfoScope("fa-IR"))
			{
				Assert.AreEqual("%31/415", ((Percentage).31415).ToString());
			}
		}

		[Test]
		public void ToString_InvalidPercentageMarkPosition_ThrowsException()
		{
			Percentage val = 0.33m;

			Assert.Catch<FormatException>
			(() =>
			{
				val.ToString("%#%");
			},
			"Input string was not in a correct format.");
		}

		[Test]
		public void ToStringWithFormat_PercentageEnGB_FormattedString()
		{
			using (new CultureInfoScope("en-GB"))
			{
				Assert.AreEqual("11.11", ((Percentage).1111).ToString("0.00"));
				Assert.AreEqual("022.22%", ((Percentage).2222).ToString("000.00%"));
				Assert.AreEqual("%033.33", ((Percentage).3333).ToString("%000.00"));
				Assert.AreEqual("44.4%", ((Percentage).4444).ToString(@"0.#\%"));
				Assert.AreEqual(@"55.6\%", ((Percentage).5555).ToString(@"0.#\\%"));
				Assert.AreEqual(@"66.7\%", ((Percentage).6666).ToString(@"0.#\\\%"));
				Assert.AreEqual(@"777.77‰", ((Percentage).77777).ToString(@"0.0#‰"));
				Assert.AreEqual(@"‰777.78", ((Percentage).77778).ToString(@"‰0.0#"));
				Assert.AreEqual(@"8888.88‱", ((Percentage).888888).ToString(@"0.0#‱"));
				Assert.AreEqual(@"‱8888.89", ((Percentage).888889).ToString(@"‱0.0#"));
			}
		}

		#endregion

		#region IEquatable tests

		/// <summary>GetHash should not fail for the test struct.</summary>
		[Test]
		public void GetHash_TestStruct_Hash()
		{
			Assert.AreEqual(2097520717, PercentageTest.TestStruct.GetHashCode());
		}

		[Test]
		public void Equals_FormattedAndUnformatted_IsTrue()
		{
			var l = Percentage.Parse("17", CultureInfo.InvariantCulture);
			var r = Percentage.Parse("17.0%", CultureInfo.InvariantCulture);

			Assert.IsTrue(l.Equals(r));
		}

		[Test]
		public void Equals_TestStructTestStruct_IsTrue()
		{
			Assert.IsTrue(PercentageTest.TestStruct.Equals(PercentageTest.TestStruct));
		}

		[Test]
		public void Equals_TestStructEmpty_IsFalse()
		{
			Assert.IsFalse(PercentageTest.TestStruct.Equals((Percentage)0.18m));
		}

		[Test]
		public void Equals_TestStructObjectTestStruct_IsTrue()
		{
			Assert.IsTrue(((Percentage)0.1751m).Equals((object)PercentageTest.TestStruct));
		}

		[Test]
		public void Equals_TestStructNull_IsFalse()
		{
			Assert.IsFalse(PercentageTest.TestStruct.Equals(null));
		}

		[Test]
		public void Equals_TestStructObject_IsFalse()
		{
			Assert.IsFalse(PercentageTest.TestStruct.Equals(new object()));
		}

		[Test]
		public void OperatorIs_TestStructTestStruct_IsTrue()
		{
			var l = PercentageTest.TestStruct;
			var r = PercentageTest.TestStruct;
			Assert.IsTrue(l == r);
		}

		[Test]
		public void OperatorIsNot_TestStructTestStruct_IsFalse()
		{
			var l = PercentageTest.TestStruct;
			var r = PercentageTest.TestStruct;
			Assert.IsFalse(l != r);
		}

		#endregion

		#region IComparable tests

		/// <summary>Orders a list of Percentages ascending.</summary>
		[Test]
		public void OrderBy_Percentage_AreEqual()
		{
			Percentage item0 = 0.0185m;
			Percentage item1 = 0.1230m;
			Percentage item2 = 0.2083m;
			Percentage item3 = 0.3333m;

			var inp = new List<Percentage>() { item3, item2, item0, item1 };
			var exp = new List<Percentage>() { item0, item1, item2, item3 };
			var act = inp.OrderBy(item => item).ToList();

			CollectionAssert.AreEqual(exp, act);
		}

		/// <summary>Orders a list of Percentages descending.</summary>
		[Test]
		public void OrderByDescending_Percentage_AreEqual()
		{
			Percentage item0 = 0.0185m;
			Percentage item1 = 0.1230m;
			Percentage item2 = 0.2083m;
			Percentage item3 = 0.3333m;

			var inp = new List<Percentage>() { item3, item2, item0, item1 };
			var exp = new List<Percentage>() { item3, item2, item1, item0 };
			var act = inp.OrderByDescending(item => item).ToList();

			CollectionAssert.AreEqual(exp, act);
		}

		/// <summary>Compare with a to object casted instance should be fine.</summary>
		[Test]
		public void CompareTo_ObjectTestStruct_0()
		{
			object other = TestStruct;

			var exp = 0;
			var act = TestStruct.CompareTo(other);

			Assert.AreEqual(exp, act);
		}

		/// <summary>Compare with null should throw an exception.</summary>
		[Test]
		public void CompareTo_null_ThrowsArgumentException()
		{
			ExceptionAssert.CatchArgumentException
			(() =>
				{
					object other = null;
					var act = TestStruct.CompareTo(other);
				},
				"obj",
				"Argument must be a percentage"
			);
		}
		/// <summary>Compare with a random object should throw an exception.</summary>
		[Test]
		public void CompareTo_newObject_ThrowsArgumentException()
		{
			ExceptionAssert.CatchArgumentException
			(() =>
				{
					object other = new object();
					var act = TestStruct.CompareTo(other);
				},
				"obj",
				"Argument must be a percentage"
			);
		}

		[Test]
		public void LessThan_17LT19_IsTrue()
		{
			Percentage l = 0.17m;
			Percentage r = 0.19m;

			Assert.IsTrue(l < r);
		}
		[Test]
		public void GreaterThan_21LT19_IsTrue()
		{
			Percentage l = 0.21m;
			Percentage r = 0.19m;

			Assert.IsTrue(l > r);
		}

		[Test]
		public void LessThanOrEqual_17LT19_IsTrue()
		{
			Percentage l = 0.17m;
			Percentage r = 0.19m;

			Assert.IsTrue(l <= r);
		}
		[Test]
		public void GreaterThanOrEqual_21LT19_IsTrue()
		{
			Percentage l = 0.21m;
			Percentage r = 0.19m;

			Assert.IsTrue(l >= r);
		}

		[Test]
		public void LessThanOrEqual_17LT17_IsTrue()
		{
			Percentage l = 0.17m;
			Percentage r = 0.17m;

			Assert.IsTrue(l <= r);
		}
		[Test]
		public void GreaterThanOrEqual_21LT21_IsTrue()
		{
			Percentage l = 21.0;
			Percentage r = 21.0;

			Assert.IsTrue(l >= r);
		}
		#endregion

		#region Percentage manipulation tests

		[Test]
		public void UnaryNegation_Percentage17_Min17()
		{
			Percentage act = 0.17m;
			Percentage exp = -0.17m;

			act = -act;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void UnaryPlus_Percentage17_17()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.17m;

			act = +act;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Multiply_Percentage17Percentage42_741()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.0714m;
			Percentage mut = 0.42m;

			act = act * mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Devide_Percentage17Percentage50_34()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.34m;
			Percentage mut = 0.50m;

			act = act / mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Add_Percentage17Percentage42_59()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.59m;
			Percentage mut = 0.42m;

			act = act + mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Subtract_Percentage17Percentage13_59()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.04m;
			Percentage mut = 0.13m;

			act = act - mut;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Multiply_Percentage17Decimal42_741()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.0714m;
			Decimal mut = 0.42m;

			act = act * mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Percentage17Double42_741()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.0714m;
			Double mut = 0.42;

			act = act * mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Percentage17Single42_741()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.0714m;
			Single mut = 0.42F;

			act = act * mut;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Multiply_Percentage17Int6442_34()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.34m;
			Int64 mut = 2;

			act = act * mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Percentage17Int2342_34()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.34m;
			Int32 mut = 2;

			act = act * mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Percentage17Int1642_34()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.34m;
			Int16 mut = 2;

			act = act * mut;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Multiply_Percentage17UInt6442_34()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.34m;
			UInt64 mut = 2;

			act = act * mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Percentage17UInt2342_34()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.34m;
			UInt32 mut = 2;

			act = act * mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Percentage17UInt1642_34()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.34m;
			UInt16 mut = 2;

			act = act * mut;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Division_Percentage17Decimal42_085()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.4047619047619047619047619048m;
			Decimal mut = 0.42m;

			act = act / mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Percentage17Double42_085()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.4047619047619047619047619048m;
			Double mut = 0.42;

			act = act / mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Percentage17Single42_085()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.4047619047619047619047619048m;
			Single mut = 0.42F;

			act = act / mut;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Division_Percentage17Int6442_085()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.085m;
			Int64 mut = 2;

			act = act / mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Percentage17Int2342_085()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.085m;
			Int32 mut = 2;

			act = act / mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Percentage17Int1642_085()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.085m;
			Int16 mut = 2;

			act = act / mut;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Division_Percentage17UInt6442_085()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.085m;
			UInt64 mut = 2;

			act = act / mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Percentage17UInt2342_085()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.085m;
			UInt32 mut = 2;

			act = act / mut;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Percentage17UInt1642_085()
		{
			Percentage act = 0.17m;
			Percentage exp = 0.085m;
			UInt16 mut = 2;

			act = act / mut;

			Assert.AreEqual(exp, act);
		}

		#endregion

		#region Number manipulation tests

		[Test]
		public void Increment_Percentage10_Percentage11()
		{
			Percentage act = 0.1m;
			Percentage exp = 0.11m;
			act++;

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Decrement_Percentage10_Percentage09()
		{
			Percentage act = 0.1m;
			Percentage exp = 0.09m;
			act--;

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Addition_Int1617Percentage10_18()
		{
			Int16 act = 17;
			Int16 exp = 18;
			act = act + Percentage.Create(0.1);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Addition_Int3217Percentage10_18()
		{
			Int32 act = 17;
			Int32 exp = 18;
			act = act + Percentage.Create(0.1);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Addition_Int6417Percentage10_18()
		{
			Int64 act = 17;
			Int64 exp = 18;
			act = act + Percentage.Create(0.1);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Addition_UInt1617Percentage50_25()
		{
			UInt16 act = 17;
			UInt16 exp = 25;
			act = act + Percentage.Create(0.5);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Addition_UInt3217Percentage50_25()
		{
			UInt32 act = 17;
			UInt32 exp = 25;
			act = act + Percentage.Create(0.5);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Addition_UInt6417Percentage50_25()
		{
			UInt64 act = 17;
			UInt64 exp = 25;
			act = act + Percentage.Create(0.5);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Addition_Decimal17Percentage17_19D89()
		{
			Decimal act = 17;
			Decimal exp = 19.89m;
			act = act + Percentage.Create(0.17);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Addition_Double17Percentage17_19D89()
		{
			Double act = 17;
			Double exp = 19.89;
			act = act + Percentage.Create(0.17);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Addition_Single17Percentage17_19D89()
		{
			Single act = 17;
			Single exp = 19.89F;
			act = act + Percentage.Create(0.17);

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Subtraction_Int1617Percentage10_16()
		{
			Int16 act = 17;
			Int16 exp = 16;
			act = act - Percentage.Create(0.1);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Subtraction_Int3217Percentage10_16()
		{
			Int32 act = 17;
			Int32 exp = 16;
			act = act - Percentage.Create(0.1);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Subtraction_Int6417Percentage10_16()
		{
			Int64 act = 17;
			Int64 exp = 16;
			act = act - Percentage.Create(0.1);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Subtraction_UInt1617Percentage50_9()
		{
			UInt16 act = 17;
			UInt16 exp = 9;
			act = act - Percentage.Create(0.5);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Subtraction_UInt3217Percentage50_9()
		{
			UInt32 act = 17;
			UInt32 exp = 9;
			act = act - Percentage.Create(0.5);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Subtraction_UInt6417Percentage50_9()
		{
			UInt64 act = 17;
			UInt64 exp = 9;
			act = act - Percentage.Create(0.5);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Subtraction_Decimal17Percentage17_11D39()
		{
			Decimal act = 17;
			Decimal exp = 11.39m;
			act = act - Percentage.Create(0.33m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Subtraction_Double17Percentage17_11D39()
		{
			Double act = 17;
			Double exp = 11.39;
			act = act - Percentage.Create(0.33m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Subtraction_Single17Percentage17_11D3899994()
		{
			Single act = 17;
			Single exp = 11.3899994F;
			act = act - Percentage.Create(0.33m);

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Division_Int1617Percentage51_33()
		{
			decimal test = (decimal)17 / .51m;
			var t = (Int16)test;

			Int16 act = 17;
			Int16 exp = 33;
			act = act / Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Int3217Percentage51_33()
		{
			Int32 act = 17;
			Int32 exp = 33;
			act = act / Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Int6417Percentage51_33()
		{
			Int64 act = 17;
			Int64 exp = 33;
			act = act / Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_UInt1617Percentage51_33()
		{
			UInt16 act = 17;
			UInt16 exp = 33;
			act = act / Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_UInt3217Percentage51_33()
		{
			UInt32 act = 17;
			UInt32 exp = 33;
			act = act / Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_UInt6417Percentage51_33()
		{
			UInt64 act = 17;
			UInt64 exp = 33;
			act = act / Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Decimal17Percentage51_33()
		{
			Decimal act = 17;
			Decimal exp = 100.0m / 3.0m;
			act = act / Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Double17Percentage51_33()
		{
			Double act = 17;
			Double exp = 100.0 / 3.0;
			act = act / Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Division_Single17Percentage51_33()
		{
			Single act = 17;
			Single exp = 100.0F / 3.0F;
			act = act / Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}

		[Test]
		public void Multiply_Int1617Percentage51_8()
		{
			decimal test = (decimal)17 * .51m;
			var t = (Int16)test;

			Int16 act = 17;
			Int16 exp = 8;
			act = act * Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Int3217Percentage51_8()
		{
			Int32 act = 17;
			Int32 exp = 8;
			act = act * Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Int6417Percentage51_8()
		{
			Int64 act = 17;
			Int64 exp = 8;
			act = act * Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_UInt1617Percentage51_8()
		{
			UInt16 act = 17;
			UInt16 exp = 8;
			act = act * Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_UInt3217Percentage51_8()
		{
			UInt32 act = 17;
			UInt32 exp = 8;
			act = act * Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_UInt6417Percentage51_8()
		{
			UInt64 act = 17;
			UInt64 exp = 8;
			act = act * Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Decimal17Percentage51_8D67()
		{
			Decimal act = 17;
			Decimal exp = 8.67m;
			act = act * Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Double17Percentage51_8D67()
		{
			Double act = 17;
			Double exp = 8.67;
			act = act * Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Multiply_Single17Percentage51_8D67()
		{
			Single act = 17;
			Single exp = 8.67F;
			act = act * Percentage.Create(0.51m);

			Assert.AreEqual(exp, act);
		}

		#endregion

		#region Casting tests

		[Test]
		public void Explicit_StringToPercentage_AreEqual()
		{
			var exp = TestStruct;
			var act = (Percentage)TestStruct.ToString();

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Explicit_PercentageToString_AreEqual()
		{
			var exp = TestStruct.ToString();
			var act = (string)TestStruct;

			Assert.AreEqual(exp, act);
		}

		#endregion

		#region Properties
		#endregion

		#region Type converter tests

		[Test]
		public void ConverterExists_Percentage_IsTrue()
		{
			TypeConverterAssert.ConverterExists(typeof(Percentage));
		}

		[Test]
		public void CanNotConvertFromInt32_Percentage_IsTrue()
		{
			TypeConverterAssert.CanNotConvertFrom(typeof(Percentage), typeof(Int32));
		}
		[Test]
		public void CanNotConvertToInt32_Percentage_IsTrue()
		{
			TypeConverterAssert.CanNotConvertTo(typeof(Percentage), typeof(Int32));
		}

		[Test]
		public void CanConvertFromString_Percentage_IsTrue()
		{
			TypeConverterAssert.CanConvertFromString(typeof(Percentage));
		}

		[Test]
		public void CanConvertToString_Percentage_IsTrue()
		{
			TypeConverterAssert.CanConvertToString(typeof(Percentage));
		}

		[Test]
		public void ConvertFromString_StringValue_TestStruct()
		{
			using (new CultureInfoScope("en-GB"))
			{
				TypeConverterAssert.ConvertFromEquals(PercentageTest.TestStruct, PercentageTest.TestStruct.ToString());
			}
		}

		[Test]
		public void ConvertFromInstanceDescriptor_Percentage_Successful()
		{
			TypeConverterAssert.ConvertFromInstanceDescriptor(typeof(Percentage));
		}

		[Test]
		public void ConvertToString_TestStruct_StringValue()
		{
			using (new CultureInfoScope("en-GB"))
			{
				TypeConverterAssert.ConvertToStringEquals(PercentageTest.TestStruct.ToString(), PercentageTest.TestStruct);
			}
		}

		#endregion

		#region IsValid tests

		[Test]
		public void IsValid_Data_IsFalse()
		{
			Assert.IsFalse(Percentage.IsValid("Complex"), "Complex");
			Assert.IsFalse(Percentage.IsValid((String)null), "(String)null");
			Assert.IsFalse(Percentage.IsValid(string.Empty), "string.Empty");
		}
		[Test]
		public void IsValid_Data_IsTrue()
		{
			Assert.IsTrue(Percentage.IsValid("%12.00"));
		}
		#endregion
	}

	[Serializable]
	public class PercentageSerializeObject
	{
		public int Id { get; set; }
		public Percentage Obj { get; set; }
		public DateTime Date { get; set; }
	}
}

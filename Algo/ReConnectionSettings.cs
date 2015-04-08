namespace StockSharp.Algo
{
	using System;

	using Ecng.Common;
	using Ecng.Serialization;

	using StockSharp.BusinessEntities;
	using StockSharp.Messages;
	using StockSharp.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	/// <summary>
	/// Настройки механизма отслеживания соединений <see cref="IConnector"/> с торговом системой.
	/// </summary>
	[DisplayNameLoc(LocalizedStrings.Str977Key)]
	[DescriptionLoc(LocalizedStrings.Str978Key)]
	[ExpandableObject]
	public class ReConnectionSettings : IPersistable
	{
		/// <summary>
		/// Настройки для подключения или экспорта.
		/// </summary>
		[DisplayNameLoc(LocalizedStrings.SettingsKey)]
		[DescriptionLoc(LocalizedStrings.Str173Key)]
		[ExpandableObject]
		public class Settings : IPersistable
		{
			internal Settings()
			{
			}

			private TimeSpan _interval = TimeSpan.FromSeconds(10);

			/// <summary>
			/// Интервал, с которым будут происходить попытки установить соединение. По умолчанию интервал равен 10 секунд.
			/// </summary>
			[CategoryLoc(LocalizedStrings.Str174Key)]
			[DisplayNameLoc(LocalizedStrings.Str175Key)]
			[DescriptionLoc(LocalizedStrings.Str176Key)]
			public TimeSpan Interval
			{
				get { return _interval; }
				set
				{
					if (value < TimeSpan.Zero)
						throw new ArgumentOutOfRangeException("value", value, LocalizedStrings.Str177);

					_interval = value;
				}
			}

			private int _attemptCount;

			/// <summary>
			/// Количество попыток установить первоначальное соединение, если оно не было установлено (тайм-аут, сетевой сбой и т.д.).
			/// По-умолчанию количество попыток равно 0. Для установление беконечного количества попыток используется значение -1.
			/// </summary>
			[CategoryLoc(LocalizedStrings.Str174Key)]
			[DisplayNameLoc(LocalizedStrings.Str178Key)]
			[DescriptionLoc(LocalizedStrings.Str179Key)]
			public int AttemptCount
			{
				get { return _attemptCount; }
				set
				{
					if (value < -1)
						throw new ArgumentOutOfRangeException("value", value, LocalizedStrings.Str177);

					_attemptCount = value;
				}
			}

			private int _reAttemptCount = 100;

			/// <summary>
			/// Количество попыток переподключиться, если соединение было утеряно в процессе работы.
			/// По-умолчанию количество попыток равно 100. Для установление беконечного количества попыток используется значение -1.
			/// </summary>
			[CategoryLoc(LocalizedStrings.Str174Key)]
			[DisplayNameLoc(LocalizedStrings.Str180Key)]
			[DescriptionLoc(LocalizedStrings.Str181Key)]
			public int ReAttemptCount
			{
				get { return _reAttemptCount; }
				set
				{
					if (value < -1)
						throw new ArgumentOutOfRangeException("value", value, LocalizedStrings.Str177);

					_reAttemptCount = value;
				}
			}

			private TimeSpan _timeOutInterval = TimeSpan.FromSeconds(30);

			/// <summary>
			/// Время ожидания успешного подключения/отключения. Если значение равно <see cref="TimeSpan.Zero"/>, то мониторинг не производится.
			/// По-умолчанию значение равно 30 секундам.
			/// </summary>
			[CategoryLoc(LocalizedStrings.Str174Key)]
			[DisplayNameLoc(LocalizedStrings.Str182Key)]
			[DescriptionLoc(LocalizedStrings.Str183Key)]
			public TimeSpan TimeOutInterval
			{
				get { return _timeOutInterval; }
				set
				{
					if (value < TimeSpan.Zero)
						throw new ArgumentOutOfRangeException("value", value, LocalizedStrings.Str177);

					_timeOutInterval = value;
				}
			}

			private WorkingTime _workingTime = new WorkingTime();

			/// <summary>
			/// Режим работы, во время которого необходимо производить подключения.
			/// Например, нет необходимости проводить подключение, когда окончены торги на бирже.
			/// </summary>
			[CategoryLoc(LocalizedStrings.Str174Key)]
			[DisplayNameLoc(LocalizedStrings.Str184Key)]
			[DescriptionLoc(LocalizedStrings.Str185Key)]
			public WorkingTime WorkingTime
			{
				get { return _workingTime; }
				set
				{
					if (value == null)
						throw new ArgumentNullException("value");

					_workingTime = value;
				}
			}

			/// <summary>
			/// Событие об успешном восстановлении соединения.
			/// </summary>
			public event Action Restored;

			/// <summary>
			/// Событие о тайм-ауте подключения.
			/// </summary>
			public event Action TimeOut;

			internal void RaiseRestored()
			{
				Restored.SafeInvoke();
			}

			internal void RaiseTimeOut()
			{
				TimeOut.SafeInvoke();
			}



			/// <summary>
			/// Загрузить настройки.
			/// </summary>
			/// <param name="storage">Хранилище настроек.</param>
			public void Load(SettingsStorage storage)
			{
				if (storage.ContainsKey("WorkingTime"))
					WorkingTime.Load(storage.GetValue<SettingsStorage>("WorkingTime"));

				Interval = storage.GetValue<TimeSpan>("Interval");
				AttemptCount = storage.GetValue<int>("AttemptCount");
				ReAttemptCount = storage.GetValue<int>("ReAttemptCount");
				TimeOutInterval = storage.GetValue<TimeSpan>("TimeOutInterval");
			}

			/// <summary>
			/// Сохранить настройки.
			/// </summary>
			/// <param name="storage">Хранилище настроек.</param>
			public void Save(SettingsStorage storage)
			{
				storage.SetValue("WorkingTime", WorkingTime.Save());
				storage.SetValue("Interval", Interval);
				storage.SetValue("AttemptCount", AttemptCount);
				storage.SetValue("ReAttemptCount", ReAttemptCount);
				storage.SetValue("TimeOutInterval", TimeOutInterval);
			}
		}

		/// <summary>
		/// Создать <see cref="ReConnectionSettings"/>.
		/// </summary>
		public ReConnectionSettings()
		{
			ConnectionSettings = new Settings();
			ExportSettings = new Settings();
		}

		/// <summary>
		/// Настройки для подключения.
		/// </summary>
		[CategoryLoc(LocalizedStrings.Str174Key)]
		[DisplayNameLoc(LocalizedStrings.Str174Key)]
		[DescriptionLoc(LocalizedStrings.ConnectionSettingsKey)]
		public Settings ConnectionSettings { get; private set; }

		/// <summary>
		/// Настройки для экспорта.
		/// </summary>
		[CategoryLoc(LocalizedStrings.Str174Key)]
		[DisplayNameLoc(LocalizedStrings.ExportKey)]
		[DescriptionLoc(LocalizedStrings.ExportSettingsKey)]
		public Settings ExportSettings { get; private set; }

		/// <summary>
		/// Режим работы, во время которого необходимо производить подключения.
		/// Например, нет необходимости проводить подключение, когда окончены торги на бирже.
		/// </summary>
		[CategoryLoc(LocalizedStrings.Str174Key)]
		[DisplayNameLoc(LocalizedStrings.Str184Key)]
		[DescriptionLoc(LocalizedStrings.Str185Key)]
		public WorkingTime WorkingTime
		{
			get { return ConnectionSettings.WorkingTime; }
			set { ConnectionSettings.WorkingTime = value; }
		}

		/// <summary>
		/// Загрузить настройки.
		/// </summary>
		/// <param name="storage">Хранилище настроек.</param>
		public void Load(SettingsStorage storage)
		{
			ConnectionSettings.Load(storage.GetValue<SettingsStorage>("ConnectionSettings"));
			ExportSettings.Load(storage.GetValue<SettingsStorage>("ExportSettings"));
		}

		/// <summary>
		/// Сохранить настройки.
		/// </summary>
		/// <param name="storage">Хранилище настроек.</param>
		public void Save(SettingsStorage storage)
		{
			storage.SetValue("ConnectionSettings", ConnectionSettings.Save());
			storage.SetValue("ExportSettings", ExportSettings.Save());
		}
	}
}
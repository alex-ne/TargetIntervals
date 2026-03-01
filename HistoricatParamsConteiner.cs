using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetIntervals
{
    /// <summary>
    /// Способ изменения даты для получения рабочего дня
    /// </summary>
    public enum ShiftToWorkDay
    {
        None,       //Не изменять 
        Before,     //Если выходной, брать последний рабочий день, предшествующий текущему 
        After,      //Если выходной, брать первый рабочий день, следующий за текущем
        Nearest     //Если выходной, брать ближайший рабочий день, до или после текущего
    }

    public enum HistoricatParamType
    {
        [Description("Разбиение по месяцам")]
        Month, 
        [Description("Разбиение по годам")]
        Year,
        [Description("Периоды постоянной процентной ставки")]
        Rate,
        [Description("Остаток ОД на дату")]
        Rest,
        [Description("Срочные выплаты по ОД")]
        Pay,
        [Description("Срочные выплаты по %")]
        Percent
    }
    public class HistoricatParam : SortedList<DateTime, double?>
    {
        public HistoricatParam(HistoricatParamType type, HistoricatParamsConteiner owner)
        {
            this._owner = owner;
            this._type = type;
        }
        // Контейнер, владеющий последовательностью
        public HistoricatParamsConteiner Owner { get { return this._owner; } }
        // Девая граница (минимальная дата) ключа последоавательности
        public DateTime Left { get; set; }
        // Правая граница (максимальная дата) ключа последоавательности
        public DateTime Right { get; set; }
        // Тип интервала
        public HistoricatParamType ListType => _type;
        // Описание интервала
        public string Description { get { return GetDescription(this._type); } }
        // Добавляем узел в последовательность
        public bool AddPoint(DateTime date, double value)
        {
            if (date < Left || date >= Right) return false;

            if (this.ContainsKey(date))
            {
                this[date] = value;
                return false;
            }
            else
            {
                this.Add(date, value);
                return true;
            }
        }
        // Исключаем узел из последовательности
        public bool RemovePoint(DateTime date)
        {
            return this.Remove(date); 
        }   
        /// <summary>
        /// Построение равномерной последовательности с заданным интервалом, 
        /// содержащей опорную дату 
        /// </summary>
        /// <param name="baseDate">Опорная дата последовательности</param>
        /// <param name="dayCount">Интервал между соседними элементами последовательности в днях</param>
        /// <param name="shiftType">Способ смещения узла последовательности, приходящегося на выходной день</param>
        public void BuildUniformSequence(DateTime baseDate, int dayCount, ShiftToWorkDay shiftType)
        {
            Clear();
            if (dayCount < 1 || baseDate < Left || baseDate >= Right) return;

            SortedList<DateTime, double?> list = new SortedList<DateTime, double?>();
            DateTime dt = baseDate;
            //Ход влево
            while (dt >= Left)
            {
                this.Add(Owner.GetWorkDay(dt, shiftType), null);
                dt = dt.AddDays(-dayCount);
            }
            //Ход вправо
            dt = baseDate.AddDays(dayCount);
            while (dt < Right)
            {
                this.Add(Owner.GetWorkDay(dt, shiftType), null);
                dt = dt.AddDays(dayCount);
            }
        }
        private static string GetDescription(HistoricatParamType value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

        private HistoricatParamsConteiner _owner;
        private HistoricatParamType _type;
    }

    public class HistoricatParamsConteiner
    {
        public HistoricatParamsConteiner(DateTime Head, DateTime Tile, Int64? maska = null)
        {
            _bounds = new DateInterval(Head, Tile);
            _historicalParamsLists = new List<HistoricatParam>();
            foreach (HistoricatParamType t in Enum.GetValues(typeof(HistoricatParamType)))
            {
                byte pos = (byte)t;
                bool permit = maska is not null
                    ? ((maska >> pos) & 1) == 1
                    : true;
                if (permit)
                {
                    _historicalParamsLists.Add(new HistoricatParam(t, this));
                }
            }
        }
        public bool? DayIsWork(DateTime day)
        {
            return OnDayStatus?.Invoke(day);
        }
        public static Func<DateTime, bool?> OnDayStatus { get; set; }
        internal DateTime GetWorkDay(DateTime target, ShiftToWorkDay shiftType)
        {
            if (shiftType == ShiftToWorkDay.None) return target;
            int delta = shiftType == ShiftToWorkDay.After ? 1 : -1;
            DateTime next = target;
            int day = 1;
            while (day <= 30)
            {
                if (shiftType == ShiftToWorkDay.Before)
                {
                    next = target.AddDays(-day);
                    if (DayIsWork(next) ?? false) return next;
                }
                else if (shiftType == ShiftToWorkDay.After)
                {
                    next = target.AddDays(day);
                    if (DayIsWork(next) ?? false) return next;
                }
                else if (shiftType == ShiftToWorkDay.Nearest)
                {
                    next = target.AddDays(-day);
                    if (DayIsWork(next) ?? false) return next;
                    next = target.AddDays(day);
                    if (DayIsWork(next) ?? false) return next;
                }
                day++;
            }
            return target;
        }

        private DateInterval _bounds;
        List<HistoricatParam> _historicalParamsLists;
    }
}

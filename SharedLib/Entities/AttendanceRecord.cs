using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    public class AttendanceRecord
    {
        public long EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; } = null!;

        public long? CenterId { get; set; }

        [ForeignKey(nameof(CenterId))]
        public Center? Center { get; set; }

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now).AddDays(1 - DateTime.Now.Day);

        public string? Comments { get; set; }

        public bool? Day01_IsAttendant { get; set; } = null;
        public long? Day01_Desc { get; set; } = null;

        public bool? Day02_IsAttendant { get; set; } = null;
        public long? Day02_Desc { get; set; } = null;

        public bool? Day03_IsAttendant { get; set; } = null;
        public long? Day03_Desc { get; set; } = null;

        public bool? Day04_IsAttendant { get; set; } = null;
        public long? Day04_Desc { get; set; } = null;

        public bool? Day05_IsAttendant { get; set; } = null;
        public long? Day05_Desc { get; set; } = null;

        public bool? Day06_IsAttendant { get; set; } = null;
        public long? Day06_Desc { get; set; } = null;

        public bool? Day07_IsAttendant { get; set; } = null;
        public long? Day07_Desc { get; set; } = null;

        public bool? Day08_IsAttendant { get; set; } = null;
        public long? Day08_Desc { get; set; } = null;

        public bool? Day09_IsAttendant { get; set; } = null;
        public long? Day09_Desc { get; set; } = null;

        public bool? Day10_IsAttendant { get; set; } = null;
        public long? Day10_Desc { get; set; } = null;

        public bool? Day11_IsAttendant { get; set; } = null;
        public long? Day11_Desc { get; set; } = null;

        public bool? Day12_IsAttendant { get; set; } = null;
        public long? Day12_Desc { get; set; } = null;

        public bool? Day13_IsAttendant { get; set; } = null;
        public long? Day13_Desc { get; set; } = null;

        public bool? Day14_IsAttendant { get; set; } = null;
        public long? Day14_Desc { get; set; } = null;

        public bool? Day15_IsAttendant { get; set; } = null;
        public long? Day15_Desc { get; set; } = null;

        public bool? Day16_IsAttendant { get; set; } = null;
        public long? Day16_Desc { get; set; } = null;

        public bool? Day17_IsAttendant { get; set; } = null;
        public long? Day17_Desc { get; set; } = null;

        public bool? Day18_IsAttendant { get; set; } = null;
        public long? Day18_Desc { get; set; } = null;

        public bool? Day19_IsAttendant { get; set; } = null;
        public long? Day19_Desc { get; set; } = null;

        public bool? Day20_IsAttendant { get; set; } = null;
        public long? Day20_Desc { get; set; } = null;

        public bool? Day21_IsAttendant { get; set; } = null;
        public long? Day21_Desc { get; set; } = null;

        public bool? Day22_IsAttendant { get; set; } = null;
        public long? Day22_Desc { get; set; } = null;

        public bool? Day23_IsAttendant { get; set; } = null;
        public long? Day23_Desc { get; set; } = null;

        public bool? Day24_IsAttendant { get; set; } = null;
        public long? Day24_Desc { get; set; } = null;

        public bool? Day25_IsAttendant { get; set; } = null;
        public long? Day25_Desc { get; set; } = null;

        public bool? Day26_IsAttendant { get; set; } = null;
        public long? Day26_Desc { get; set; } = null;

        public bool? Day27_IsAttendant { get; set; } = null;
        public long? Day27_Desc { get; set; } = null;

        public bool? Day28_IsAttendant { get; set; } = null;
        public long? Day28_Desc { get; set; } = null;

        public bool? Day29_IsAttendant { get; set; } = null;
        public long? Day29_Desc { get; set; } = null;

        public bool? Day30_IsAttendant { get; set; } = null;
        public long? Day30_Desc { get; set; } = null;

        public bool? Day31_IsAttendant { get; set; } = null;
        public long? Day31_Desc { get; set; } = null;
    }
}

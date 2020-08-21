using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWDM_Tx_4x25
{
    public class CTestData_Channel
    {
        #region Properties

        public int Temp_id;
        public int Channel;
        private double _vcpa;
        public double Vcpa
        {

            get
            {
                return _vcpa;
            }
            set
            {
                _vcpa = Math.Round(value, 2);
            }
        }
        private double _veq;
        public double Veq
        {
            get
            {
                return _veq;
            }
            set
            {
                _veq = Math.Round(value, 2);
            }
        }
        private double _vmod;
        public double Vmod
        {
            get
            {
                return _vmod;
            }
            set
            {
                _vmod = Math.Round(value, 2);
            }
        }
        private double _isink;
        public double Isink
        {
            get
            {
                return _isink;
            }
            set
            {
                _isink = Math.Round(value, 2);
            }
        }
        private double _ldd;

        public double Ldd
        {
            get
            {
                return _ldd;
            }
            set
            {
                _ldd = Math.Round(value, 2);
            }
        }
        private double _power;
        public double Power
        {
            get
            {
                return _power;
            }
            set
            {
                _power = Math.Round(value, 2);
            }
        }
        private double _impd;
        public double Impd
        {
            get
            {
                return _impd;
            }
            set
            {
                _impd = Math.Round(value, 2);
            }
        }
        private double _idark;
        public double Idark
        {
            get
            {
                return _idark;
            }
            set
            {
                _idark = Math.Round(value, 2);
            }
        }
        private double _cwl;
        public double Cwl
        {
            get
            {
                return _cwl;
            }
            set
            {
                _cwl = Math.Round(value, 2);
            }
        }
        private double _smsr;
        public double SMSR
        {
            get
            {
                return _smsr;
            }
            set
            {
                _smsr = Math.Round(value, 2);
            }
        }
        private double _jitter_pp;
        public double Jitter_pp
        {
            get
            {
                return _jitter_pp;
            }
            set
            {
                _jitter_pp = Math.Round(value, 2);
            }
        }
        private double _jitter_rms;

        public double Jitter_rms
        {
            get
            {
                return _jitter_rms;
            }
            set
            {
                _jitter_rms = Math.Round(value, 2);
            }
        }
        private double _crossing;
        public double Crossing
        {
            get
            {
                return _crossing;
            }
            set
            {
                _crossing = Math.Round(value, 2);
            }
        }
        private double _fall_time;

        public double Fall_time
        {
            get
            {
                return _fall_time;
            }
            set
            {
                _fall_time = Math.Round(value, 2);
            }
        }
        private double _rise_time;

        public double Rise_time
        {
            get
            {
                return _rise_time;
            }
            set
            {
                _rise_time = Math.Round(value, 2);
            }
        }
        private double _er;

        public double Er
        {
            get
            {
                return _er;
            }
            set
            {
                _er = Math.Round(value, 2);
            }
        }
    private double _mask_margin;

        public double Mask_Margin
        {
            get
            {
                return _mask_margin;
            }
            set
            {
                _mask_margin = Math.Round(value, 2);
            }
        }
        private double pre_cursor;

        public double Pre_Cursor
        {
            get
            {
                return pre_cursor;
            }
            set
            {
                pre_cursor = Math.Round(value, 2);
            }
        }
        private double main_cursor;

        public double Main_Cursor
        {
            get
            {
                return main_cursor;
            }
            set
            {
                main_cursor = Math.Round(value, 2);
            }
        }
        private double post_cursor;

        public double Post_Cursor
        {
            get
            {
                return post_cursor;
            }
            set
            {
                post_cursor = Math.Round(value, 2);
            }
        }
        public bool Pf
        {
            get;
            set;
        }
        #endregion
    }
}

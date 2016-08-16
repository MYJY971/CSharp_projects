using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MYTestOpenGl
{
    public class RotationMatrix : Matrix 
    {
        private int _axe;
        public RotationMatrix (int axe, double angle)
            :base(new float[3,3] )
        {
            this._axe = axe;

            Update(angle);

        }

        public void Update(double angle)
        {
            if (_axe == 0) //axe x
            {
                this.SetValues(new float[3, 3] { {1, 0,0 },
                                                 {0,(float)Math.Cos(angle), (float)-Math.Sin(angle) },
                                                 {0, (float)Math.Sin(angle),(float)Math.Cos(angle) } });
            }

            if (_axe == 1) //axe y
            {
                this.SetValues(new float[3, 3] { {(float)Math.Cos(angle), 0, (float)Math.Sin(angle) },
                                                 {0, 1, 0 },
                                                 {(float)-Math.Sin(angle), 0, (float)Math.Cos(angle) } });
            }

            if (_axe == 2) //axe z
            {
                this.SetValues(new float[3, 3] { {(float)Math.Cos(angle), (float)-Math.Sin(angle), 0 },
                                                 {(float)Math.Sin(angle), (float)Math.Cos(angle), 0 },
                                                 {0, 0, 1 } });
            }

        }
    }
}

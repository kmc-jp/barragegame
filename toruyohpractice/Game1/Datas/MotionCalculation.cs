using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class MotionCalculation
    {
        public static Vector sinWaveDisplacement(double speed,int all_time,int now_time)
        {
            //横幅はspeed*all_time/2
            double x, y;
            //double omega = 4 * Math.PI/ all_time;
            x = speed;
            y = (speed*all_time)/4 * (2 * Math.PI / all_time) * Math.Cos(2 * Math.PI * now_time / all_time );
            
            return new Vector(x, y);
        }

        public static Vector mugenDisplacement(double speed,int all_time,int now_time)
        {
            Vector displacement;
            if (now_time <= all_time / 2)
            {
                displacement = sinWaveDisplacement(speed, all_time/2, now_time);
            }
            else
            {
                displacement = sinWaveDisplacement(speed, all_time/2, now_time);
                displacement.X *= -1;
            }
            return displacement;
        }
        /// <summary>
        /// default_angleのdefaultは一番上から
        /// </summary>>
        /// <returns></returns>
        /// <param name="r"></param>
        /// <param name="now_time"></param>
        /// <param name="default_angle"></param
        public static Vector rightcircleDisplacement(double speed,int all_time, int now_time,double default_angle=-Math.PI/2)
        {
            double omega = 2 * Math.PI / all_time;
            double r = speed * omega;
            return new Vector(-r * omega * Math.Sin(omega * now_time + default_angle), r * Math.Cos(omega * now_time + default_angle));
        }
        public static Vector leftcircleDisplacement(double speed,int all_time, int now_time, double default_angle = Math.PI / 2)
        {
            double omega = 2 * Math.PI / all_time;
            double r = speed * omega;
            return new Vector(r * omega * Math.Sin(-(omega * now_time + default_angle)), -r * Math.Cos(-(omega * now_time + default_angle)));
        }

        public static Vector tousokuidouDisplacement(Vector displacement,int alltime)
        {
            return new Vector(displacement.X / alltime, displacement.Y / alltime);   
        }
    }
}

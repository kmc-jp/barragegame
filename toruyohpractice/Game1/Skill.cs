using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    enum Timing { moving, dying,  } //movingは普通の状態、dyingは死んだ瞬間んを意味する
    class Skill
    {
        public int coolDown=0;
        public string skillName;

        public Skill(string _skillName)
        {
            skillName = _skillName;
        }

        public void update(bool update=true)
        {
            if (update)
            {
                coolDown--;
            }
        }

        public bool used( Timing timing,int loop_index=-1,int hp = 0, double hpP =0)
        {
            bool succeed = false;
            bool afterDeath= skillName.Contains(DataBase.skillUsedAfterDeath);
            int routeConditionIndex = skillName.LastIndexOf(DataBase.Routine);
            bool LoopIndexCorrect;
            if (routeConditionIndex == -1)
            {
                LoopIndexCorrect = true;
            }else if (loop_index >= 0)
            {
                routeConditionIndex += DataBase.Routine.Length;
                
                while ( routeConditionIndex<skillName.Length && 
                    skillName[routeConditionIndex]-'0'>=0  
                    ) {

                }
            }else { LoopIndexCorrect = false; }
            
            switch (timing)
            {
                case Timing.moving:
                    succeed = !afterDeath;
                    break;
                case Timing.dying:
                    succeed = afterDeath;
                    break;
            }
            if (succeed) {
                coolDown = DataBase.getSkillData(skillName).cooldownFps;
            }
            return succeed;
        }
    }
}

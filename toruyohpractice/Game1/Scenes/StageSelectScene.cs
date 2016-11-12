using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class StageSelectScene:Scene
    {
        /// <summary>
        /// indexではない
        /// </summary>
        int stage_select =1;
        public int stage_number = 5;
        Vector player_pos = new Vector();
        Vector stage1_pos = new Vector(450, 450);
        Vector stage2_pos = new Vector(100, 200);
        Vector stage3_pos = new Vector(400, 80);
        Vector stage4_pos = new Vector(750, 150);
        Vector stage5_pos = new Vector(1080, 25);


        public StageSelectScene(SceneManager scenem) : base(scenem) { }

        public override void SceneUpdate()
        {
            if (Input.GetKeyPressed(KeyID.Up) == true || Input.GetKeyPressed(KeyID.Right) == true)
            {
                stage_select++;
            }
            if (Input.GetKeyPressed(KeyID.Down) == true || Input.GetKeyPressed(KeyID.Left) == true)
            {
                stage_select--;
            }
            if (stage_select >= stage_number){ stage_select = stage_number;}
            if (stage_select <= 1){stage_select = 1;}

            if (Input.GetKeyPressed(KeyID.Select) == true)
            {
                new MapScene(scenem,stage_select);
            }

            if (Input.GetKeyPressed(KeyID.Escape) == true)
            {
                ;
            }

            switch (stage_select)
            {
                case 1:
                    player_pos = stage1_pos;
                    break;
                case 2:
                    player_pos = stage2_pos;
                    break;
                case 3:
                    player_pos = stage3_pos;
                    break;
                case 4:
                    player_pos = stage4_pos;
                    break;
                case 5:
                    player_pos = stage5_pos;
                    break;
                default:
                    break;
            }
            
        }

        public override void SceneDraw(Drawing d)
        {
            d.Draw(new Vector(0, 0), DataBase.getTex("stageselect"), DepthID.BackGroundWall);
            d.Draw(player_pos, DataBase.getTex("130 149-player"), DepthID.Player);
        }

        public int stage_decide()
        {
            return stage_select;
        }

    }
}

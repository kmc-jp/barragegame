using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPart
{
    class StageSelectScene:Scene
    {
        int stage_select =1;
        public int stage_number = 5;
        Vector player_pos = new Vector();
        Vector stage1_pos = new Vector(100, 600);
        Vector stage2_pos = new Vector(200, 500);
        Vector stage3_pos = new Vector(300, 400);
        Vector stage4_pos = new Vector(400, 300);
        Vector stage5_pos = new Vector(500, 200);


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
            d.Draw(new Vector(0, 0), DataBase.getTex("testbackground.png"), DepthID.BackGroundWall);
            d.Draw(player_pos, DataBase.getTex("60 105-player.png"), DepthID.Player);
        }

        public int stage_decide()
        {
            return stage_select;
        }

    }
}

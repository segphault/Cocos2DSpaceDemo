using System;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace Cocos2DSpaceDemo
{
    public class IntroLayer : CCLayer
    {
		CCSize WinSize { get { return CCDirector.SharedDirector.WinSize; } }

        public IntroLayer()
        {
			AddChild(new CCParticleRain {
				Rotation = 90f, StartSize = 7f,
				Position = new CCPoint(WinSize.Width, WinSize.Center.Y)
			});

			AddChild (new CCLabelTTF ("Space Game", "MarkerFelt", 22) {
				Position = new CCPoint (WinSize.Center.X, WinSize.Height - 50)
			});

			AddChild (new CCLabelTTF ("Tap Screen to Start Game", "MarkerFelt", 16) {
				Position = WinSize.Center,
				Color = new CCColor3B (0, 0, 255)
			});

			CCDirector.SharedDirector.TouchDispatcher.AddTargetedDelegate (this, 0, true);
		}

		public override bool TouchBegan (CCTouch touch)
		{
			CCDirector.SharedDirector.ReplaceScene (GameLayer.Scene);
			return true;
		}

        public static CCScene Scene
        {
            get
            {
                var scene = new CCScene();
                var layer = new IntroLayer();

                scene.AddChild(layer);

                return scene;
            }

        }
    }
}


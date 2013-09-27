using System;
using System.Collections.Generic;

using Cocos2D;

namespace Cocos2DSpaceDemo
{
	public class GameLayer : CCLayer
	{
		Random random = new Random ();

		CCSize WinSize { get { return CCDirector.SharedDirector.WinSize; } }

		CCSprite rocket;
		CCLabelTTF labelScore;
		CCAnimation saucerAnimation;

		int playerScore = 0;
		bool gameOver = false;

		int updates = 0;

		public GameLayer ()
		{
			AddChild(new CCParticleRain {
				Rotation = 90f, StartSize = 7f,
				Position = new CCPoint(WinSize.Width, WinSize.Center.Y)
			});

			labelScore = new CCLabelTTF ("Points: 0", "MarkerFelt", 16) {
				Position = new CCPoint (WinSize.Width - 50, 10),
				Color = new CCColor3B (255, 255, 0)
			};

			AddChild (labelScore);

			var saucerTexture = new CCTexture2D ();
			saucerTexture.InitWithFile ("saucer.png");
			saucerAnimation = GenerateAnimation (saucerTexture, 6, 40, 30, 0.5f);

			var rocketTexture = new CCTexture2D ();
			rocketTexture.InitWithFile ("spaceship.png");
			var rocketAnimation = GenerateAnimation (rocketTexture, 4, 64, 29, 0.05f);
			var rocketAnimationAction = new CCRepeatForever (new CCAnimate (rocketAnimation));

			rocket = new CCSprite {
				Position = new CCPoint (WinSize.Center.X / 2, WinSize.Center.Y)
			};

			rocket.RunAction (rocketAnimationAction);
			AddChild (rocket);

			TouchEnabled = true;
			ScheduleUpdate ();

			CCDirector.SharedDirector.TouchDispatcher.AddTargetedDelegate (this, 0, true);
		}

		public void CollectStar (CCNode node)
		{
			node.RemoveFromParentAndCleanup (true);

			AddChild (new CCParticleExplosion {
				Position = node.Position,
				Life = 0.5f, LifeVar = 1f, StartSize = 0.1f,
				StartColor = new CCColor4F (255, 255, 0, 1f),
				AutoRemoveOnFinish = true
			});

			labelScore.Text = String.Format ("Points: {0}", ++playerScore);
		}

		public void GameOver (CCNode node)
		{
			rocket.Position = new CCPoint (-100, -100);

			node.RemoveFromParentAndCleanup (true);
			rocket.RemoveFromParentAndCleanup (true);

			AddChild (new CCParticleExplosion {
				Position = node.Position,
				AutoRemoveOnFinish = true,
				StartColor = new CCColor4F (255, 0, 0, 1f)
			});

			AddChild (new CCLabelTTF ("Game Over!", "MarkerFelt", 16) {
				Position = WinSize.Center,
				Color = new CCColor3B (255, 0, 0)
			});

			gameOver = true;
		}

		void HandleCollision (CCNode node)
		{
			if ((string)node.UserData == "Star")
				CollectStar (node);

			if ((string)node.UserData == "Saucer")
				GameOver (node);
		}

		public override void Update (float dt)
		{
			foreach (var child in Children)
				if (rocket.BoundingBox.IntersectsRect (child.BoundingBox))
					HandleCollision (child);

			updates++;

			if (random.Next (100) == 1) {
				var saucerY = random.Next((int)WinSize.Height);

				var saucer = new CCSprite {
					Position = new CCPoint(WinSize.Width, saucerY),
					UserData = "Saucer"
				};
	
				saucer.RunAction (new CCRepeatForever (new CCAnimate (saucerAnimation)));
				saucer.RunAction (new CCSequence (
					new CCMoveTo (5f, new CCPoint (0, saucerY)),
					new CCCallFunc(saucer.RemoveFromParent)));

				AddChild (saucer);
			}

			if (random.Next (200) == 1) {
				var starY = random.Next((int)WinSize.Height);

				var star = new CCSprite("star.png") {
					Position = new CCPoint(WinSize.Width, starY),
					UserData = "Star"
				};

				star.RunAction (new CCRepeatForever (
					new CCSequence (
						new CCScaleTo (0.5f, 1.3f),
						new CCScaleTo (0.5f, 1.0f))));

				star.RunAction (new CCSequence (
					new CCMoveTo (5f, new CCPoint (0, starY)),
					new CCCallFunc (star.RemoveFromParent)));

				AddChild (star);
			}
		}

		CCAnimation GenerateAnimation(CCTexture2D texture, int frameCount, int width, int height, float duration)
		{
			var frames = new List<CCSpriteFrame> ();

			for (var i = 0; i < frameCount; i++)
				frames.Add (new CCSpriteFrame (
					texture, new CCRect (0, i * height,width, height)));

			return new CCAnimation (frames, duration);
		}

		public override bool TouchBegan (CCTouch touch)
		{
			if (gameOver)
				CCDirector.SharedDirector.ReplaceScene (IntroLayer.Scene);

			// Might be some retina issues here...
			var position = touch.Location.Y; // ConvertTouchToNodeSpaceAr (touch).Y * 2;
			rocket.RunAction(new CCMoveTo(0.5f, new CCPoint(rocket.PositionX, position)));

			return true;
		}

		public static CCScene Scene
		{
			get
			{
				var scene = new CCScene();
				var layer = new GameLayer();

				scene.AddChild(layer);

				return scene;
			}

		}
	}
}


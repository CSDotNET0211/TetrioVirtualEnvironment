﻿using System.Diagnostics;
using System.Dynamic;
using System.Numerics;
using System.Security.Claims;
using System.Text.Json;
using TetrioVirtualEnvironment.System;
using TetrReplayLoader;
using TetrReplayLoader.JsonClass;
using TetrReplayLoader.JsonClass.Event;

namespace TetrioVirtualEnvironment
{
	public class Environment
	{
		public ClassManager ClassManager;
		public class ConstData
		{
			/// <summary>
			/// This is used to judge spin bonus.
			/// Basically, used as Tspin.
			/// AllSpin uses all data.
			/// </summary>
			internal static readonly Vector2[][][] CORNER_TABLE =
			  {
              //Z
              new[]
			  {
				new[]{
					new Vector2(-2, -1), new Vector2(1, -1), new Vector2(2, 0), new Vector2(-1, 0)
				},
				  new[] {
					new Vector2(0, -1), new Vector2(1, -2), new Vector2(0, 2), new Vector2(1, 2)
				},
				  new[] {
					new Vector2(-2, 0), new Vector2(1, 0), new Vector2(2, 1), new Vector2(-1, 1)
				},
				  new[] {
					new Vector2(-1, -1), new Vector2(0, -2), new Vector2(0, 1), new Vector2(-1, 2)
				},
			  },

                //L
              new[]
			  {
				  new[] {
					  new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, 1), new Vector2(-1, 1)
				  },
				  new[] {
					  new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 0), new Vector2(-1, 1)
				  },
				  new[] {
					  new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(0, 1)
				  },
				  new[] {
					  new Vector2(-1, 0), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1)
				  },
			  },

              
              //S
              new[]
			  {
					new[] {
						  new Vector2(-1, -1), new Vector2(2, -1), new Vector2(1,0), new Vector2(-2, 0)
					},
					new[] {
						   new Vector2(0, -2), new Vector2(1, -1), new Vector2(1, 2), new Vector2(0, 1)
					},
					new[] {
						 new Vector2(-1, 0), new Vector2(2, 0), new Vector2(1, 1), new Vector2(-2,1)
					},
				   new[] {
					new Vector2(-1,-2), new Vector2(0, -1), new Vector2(-1, 1), new Vector2(0, 2)
					},
			  },
              
              //J
              new[]
			  {
					new[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
					new[] { new Vector2(-1, -1), new Vector2(1, 0), new Vector2(1, 1), new Vector2(-1, 1) },
					new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(0, 1), new Vector2(-1, 1) },
					new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 0) },
			  },
              
              //T
              new[]
			  {
				   new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
				   new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
				   new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
				   new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) },
				   },





		};

			internal static readonly Vector2[][] CORNER_ADDITIONAL_TABLE =
				   {
				   new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
				   new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
				   new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
				   new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
			   };
			/// <summary>
			/// Tetrimino Array
			/// </summary>
			public static readonly Vector2[][][] TETRIMINOS_SHAPES =
			  {
              //Z
              new[]
			  {
				new[]{
					new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1)
				},
				  new[] {
					new Vector2(2, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2)
				},
				  new[] {
					new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2)
				},
				  new[] {
					new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 2)
				},
			  },

                //L
              new[]
			  {
				  new[] {
					  new Vector2(2, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1)
				  },
				  new[] {
					  new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2), new Vector2(2, 2)
				  },
				  new[] {
					  new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 2)
				  },
				  new[] {
					  new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2)
				  },
			  },

                //O
              new[]
			  {
					 new[] {
						 new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
					 },
					new[] {
						new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
					},
					 new[]
					 { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
					 },
				   new[] {
					   new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
				   },
			  },
              
              //S
              new[]
			  {
					new[] {
						  new Vector2(1, 0), new Vector2(2, 0), new Vector2(0, 1), new Vector2(1, 1)
					},
					new[] {
						   new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 2)
					},
					new[] {
						 new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 2), new Vector2(1, 2)
					},
				   new[] {
					new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2)
					},
			  },
              
              //I
              new[]
			  {
					new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(3, 1) },
					new[] { new Vector2(2, 0), new Vector2(2, 1), new Vector2(2, 2), new Vector2(2, 3) },
					new[] { new Vector2(0, 2), new Vector2(1, 2), new Vector2(2, 2), new Vector2(3, 2) },
					new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2), new Vector2(1, 3) },
			  },
              
              
              //J
              new[]
			  {
					new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) },
					new[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(1, 1), new Vector2(1, 2) },
					new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 2) },
					new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 2), new Vector2(1, 2) },
			  },
              
              //T
              new[]
			  {
				   new[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) },
				   new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2) },
				   new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2) },
				   new[] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2) },
			  },





		};
			/// <summary>
			/// Diff Position for Fix Tetrimino Position
			/// </summary>
			public static readonly Vector2[] TETRIMINO_DIFFS = { new(1, 1), new(1, 1), new(0, 1), new(1, 1), new(1, 1), new(1, 1), new(1, 1) };
			/// <summary>
			/// for next generating
			/// </summary>
			internal static readonly int[] MINOTYPES = { (int)MinoKind.Z, (int)MinoKind.L, (int)MinoKind.O, (int)MinoKind.S, (int)MinoKind.I, (int)MinoKind.J, (int)MinoKind.T, };
			/// <summary>
			/// Kickset of SRS+
			/// </summary>
			internal static Dictionary<string, Vector2[]> KICKSET_SRSPLUS { get; } = new()
		{
			{"01", new[] { new Vector2(-1,0),new Vector2(-1,-1),new Vector2(0,2),new Vector2(-1,2),}},
			{"10", new[] { new Vector2(1,0),new Vector2(1,1),new Vector2(0,-2),new Vector2(1,-2),}},
			{"12", new[] { new Vector2(1,0),new Vector2(1,1),new Vector2(0,-2),new Vector2(1,-2),}},
			{"21", new[] { new Vector2(-1,0),new Vector2(-1,-1),new Vector2(0,2),new Vector2(-1,2),}},
			{"23", new[] { new Vector2(1,0),new Vector2(1,-1),new Vector2(0,2),new Vector2(1,2),}},
			{"32", new[] { new Vector2(-1,0),new Vector2(-1,1),new Vector2(0,-2),new Vector2(-1,-2),}},
			{"30", new[] { new Vector2(-1,0),new Vector2(-1,1),new Vector2(0,-2),new Vector2(-1,-2),}},
			{"03", new[] { new Vector2(1,0),new Vector2(1,-1),new Vector2(0,2),new Vector2(1,2),}},
			{"02", new[] { new Vector2(0,-1),new Vector2(1,-1),new Vector2(-1,-1),new Vector2(1,0),new Vector2(-1,0),}},
			{"13", new[] { new Vector2(1,0),new Vector2(1,-2),new Vector2(1,-1),new Vector2(0,-2),new Vector2(0,-1),}},
			{"20", new[] { new Vector2(0,1),new Vector2(-1,1),new Vector2(1,1),new Vector2(-1,0),new Vector2(1,0),}},
			{"31", new[] { new Vector2(-1,0),new Vector2(-1,-2),new Vector2(-1,-1),new Vector2(0,-2),new Vector2(0,-1),}}
		};
			/// <summary>
			/// Kickset of SRS+ I-piece
			/// </summary>
			internal static Dictionary<string, Vector2[]> KICKSET_SRSPLUSI { get; } = new()
		{
		{"01",new[]{new Vector2(1,0),new Vector2(-2,0),new Vector2(-2,1),new Vector2(1,-2),}},
		{"10",new[]{new Vector2(-1,0),new Vector2(2,0),new Vector2(-1,2),new Vector2(2,-1),}},
		{"12",new[]{new Vector2(-1,0),new Vector2(2,0),new Vector2(-1,-2),new Vector2(2,1),}},
		{"21",new[]{new Vector2(-2,0),new Vector2(1,0),new Vector2(-2,-1),new Vector2(1,2),}},
		{"23",new[]{new Vector2(2,0),new Vector2(-1,0),new Vector2(2,-1),new Vector2(-1,2),}},
		{"32",new[]{new Vector2(1,0),new Vector2(-2,0),new Vector2(1,-2),new Vector2(-2,1),}},
		{"30",new[]{new Vector2(1,0),new Vector2(-2,0),new Vector2(1,2),new Vector2(-2,-1),}},
		{"03",new[]{new Vector2(-1,0),new Vector2(2,0),new Vector2(2,1),new Vector2(-1,-2),}},
		{"02",new[]{new Vector2(0,-1)}},
		{"13",new[]{new Vector2(1,0)}},
		{"20",new[]{new Vector2(0,1)}},
		{"31",new[]{new Vector2(-1,0)}},
		};
		}

		public const int FIELD_WIDTH = 10;
		public const int FIELD_HEIGHT = 40;

		public static int FIELD_SIZE => FIELD_WIDTH * FIELD_HEIGHT;

		// --- events ---
		public event EventHandler? OnPiecePlaced;
		public event EventHandler? OnPieceCreated;

		// --- public propety ---
		//DOTO: これとかapmに関する情報は関数でも作る pressedkeylist中身あったっけ
		/// <summary>
		/// Pressed key data
		/// </summary>
		public List<string> PressedKeyList { get; private set; } = new();
		public int GeneratedRngCount { get; private set; }
		public bool InfinityHold { get; set; } = false;
		public string? Username { get; }

		public List<int> GarbageIDs { get; internal set; }
		public GameData GameData => ClassManager.GameData;
		/// <summary>
		/// RNG Generator
		/// </summary>
		public Rng Rng { get; } = new();
		public NextGenerateKind NextGenerateMode { get; }
		public int NextSkipCount;
		/// <summary>
		/// for process events at same frame.
		/// </summary>
		public int CurrentIndex { get; private set; }
		public int CurrentFrame { get; private set; }

		public bool IsDead { get; internal set; } = false;



		/// <summary>
		/// event data of 'full'.use it for reset the game.
		/// full event is first event after game started existing in ttr/ttrm.
		/// </summary>
		internal EventFull EventFull { get; }
		/// <summary>
		/// initialize game data. it is used in NextGenerateKind is Array.
		/// 
		/// </summary>
		internal DataForInitialize DataForInitialize { get; }

		/// <summary>
		/// ArrayMode only use initialized Next.
		/// SeedMode generate next from seed based on TETR.IO generate system.
		/// </summary>
		public enum NextGenerateKind : byte
		{
			Array,
			Seed
		}
		public enum MinoKind : sbyte
		{
			Z,
			L,
			O,
			S,
			I,
			J,
			T,
			Garbage,
			Empty,
			None = -1
		}

		/// <summary>
		/// for AI moving
		/// </summary>
		public enum Action : byte
		{
			Null,
			MoveUp,
			MoveDown,
			MoveRight,
			MoveLeft,
			RotateRight,
			RotateLeft,
			Rotate180,
			Harddrop,
			QuickSoftdrop,
			Softdrop,
			Hold
		}

		public enum KeyEvent
		{
			KeyDown,
			KeyUp
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="envData"></param>
		/// <param name="envMode"></param>
		/// <param name="username"></param>
		/// <param name="dataForInitialize"></param>
		/// <param name="nextSkipCount"></param>
		public Environment(EventFull envData, NextGenerateKind envMode, string? username, DataForInitialize? dataForInitialize = null, int nextSkipCount = 0)
		{
			ClassManager = new ClassManager
			{
				Environment = this,
				GameData = new GameData()
			};

			dataForInitialize ??= new DataForInitialize();


			Username = username;
			NextGenerateMode = envMode;
			EventFull = envData;
			DataForInitialize = dataForInitialize;
			NextSkipCount = nextSkipCount;

			ResetGame(envData, envMode, dataForInitialize, nextSkipCount);

		}

		/// <summary>
		/// ResetGame for rewind
		/// </summary>
		/// <param name="envData"></param>
		/// <param name="envMode"></param>
		/// <param name="dataForInitialize"></param>
		/// <param name="nextSkipCount"></param>
		internal void ResetGame(EventFull envData, NextGenerateKind envMode, DataForInitialize dataForInitialize, int nextSkipCount = 0)
		{
			IsDead = false;
			GeneratedRngCount = 0;
			GarbageIDs = new List<int>();
			CurrentFrame = 0;
			CurrentIndex = 0;

			ClassManager.GameData.Init(envMode, envData, ClassManager, nextSkipCount, dataForInitialize);

		}


		/// <summary>
		/// KeyInput for Self Control
		/// </summary>
		/// <param name="keyEvent"></param>
		/// <param name="keyKind"></param>
		public void InputKeyEvent(KeyEvent keyEvent, Action keyKind)
		{
			if (NextGenerateMode == NextGenerateKind.Array)
				if (keyKind == Action.Hold)
				{

				}
				else
					return;

			if (keyEvent == KeyEvent.KeyUp)
			{

			}
			else if (keyEvent == KeyEvent.KeyDown)
			{

			}
		}




		public void KeyInput(string type, EventKeyInput @event)
		{
			if (type == "keydown")
			{
				if (@event.subframe > ClassManager.GameData.SubFrame)
				{
					MovementSystem.ProcessShift(false, @event.subframe - ClassManager.GameData.SubFrame, ClassManager.GameData);
					FallEvent(null, @event.subframe - ClassManager.GameData.SubFrame);
					ClassManager.GameData.SubFrame = @event.subframe;
				}

				if (@event.key == "moveLeft")
				{
					ClassManager.GameData.LShift = true;
					ClassManager.GameData.LastShift = -1;
					ClassManager.GameData.LDas = @event.hoisted ?
						ClassManager.GameData.Handling.DAS - ClassManager.GameData.Handling.DCD : 0;
					if (ClassManager.GameData.Options.Version >= 12)
						ClassManager.GameData.LDasIter = ClassManager.GameData.Handling.ARR;

					MovementSystem.ProcessLShift(true, ClassManager.GameData, ClassManager.GameData.Options.Version >= 15 ? 0 : 1);
					return;
				}

				if (@event.key == "moveRight")
				{
					ClassManager.GameData.RShift = true;
					ClassManager.GameData.LastShift = 1;
					ClassManager.GameData.RDas = @event.hoisted ? ClassManager.GameData.Handling.DAS - ClassManager.GameData.Handling.DCD : 0;
					if (ClassManager.GameData.Options.Version >= 12)
						ClassManager.GameData.RDasIter = ClassManager.GameData.Handling.ARR;

					MovementSystem.ProcessRShift(true, ClassManager.GameData, ClassManager.GameData.Options.Version >= 15 ? 0 : 1);
					return;
				}

				if (@event.key == "softDrop")
				{
					ClassManager.GameData.SoftDrop = true;
					return;
				}

				if (ClassManager.GameData.Falling.Sleep || ClassManager.GameData.Falling.DeepSleep)
				{
					throw new NotImplementedException();
				}
				else
				{
					if (@event.key == "rotateCCW")
					{
						var e = ClassManager.GameData.Falling.R - 1;
						if (e < 0)
							e = 3;
						RotateSystem.RotatePiece(e, ClassManager.GameData);
					}

					if (@event.key == "rotateCW")
					{
						var e = ClassManager.GameData.Falling.R + 1;
						if (e > 3)
							e = 0;
						RotateSystem.RotatePiece(e, ClassManager.GameData);
					}

					if (@event.key == "rotate180" && ClassManager.GameData.Options.Allow180)
					{
						var e = ClassManager.GameData.Falling.R + 2;
						if (e > 3)
							e -= 4;
						RotateSystem.RotatePiece(e, ClassManager.GameData);
					}
					if (@event.key == "hardDrop" && ClassManager.GameData.Options.AllowHardDrop &&
						ClassManager.GameData.Falling.SafeLock == 0)
					{
						FallEvent(int.MaxValue, 1);
					}

					if (@event.key == "hold")
					{
						if (!ClassManager.GameData.HoldLocked || InfinityHold)
						{
							if ((ClassManager.GameData.Options.DisplayHold == null ||
							(bool)ClassManager.GameData.Options.DisplayHold))
								SwapHold();
						}
					}
				}

			}
			else if (type == "keyup")
			{
				if (@event.subframe > ClassManager.GameData.SubFrame)
				{
					MovementSystem.ProcessShift(false, @event.subframe - ClassManager.GameData.SubFrame, ClassManager.GameData);
					FallEvent(null, @event.subframe - ClassManager.GameData.SubFrame);
					ClassManager.GameData.SubFrame = @event.subframe;
				}

				if (@event.key == "moveLeft")
				{
					ClassManager.GameData.LShift = false;
					ClassManager.GameData.LDas = 0;

					if (ClassManager.GameData.Handling.Cancel)
					{
						ClassManager.GameData.RDas = 0;
						ClassManager.GameData.RDasIter = ClassManager.GameData.Handling.ARR;
					}

					return;
				}

				if (@event.key == "moveRight")
				{
					ClassManager.GameData.RShift = false;
					ClassManager.GameData.RDas = 0;

					if (ClassManager.GameData.Handling.Cancel)
					{
						ClassManager.GameData.LDas = 0;
						ClassManager.GameData.LDasIter = ClassManager.GameData.Handling.ARR;
					}

					return;
				}

				if (@event.key == "softDrop")
					ClassManager.GameData.SoftDrop = false;


			}
		}




		/// <summary>
		/// Update game. Don't call if environment is managed by Replay class.
		/// </summary>
		/// <param name="events"></param>
		/// <returns></returns>
		public bool Update(IReadOnlyList<Event>? events = null)
		{
			ClassManager.GameData.SubFrame = 0;

			if (events != null && !PullEvent(events))
				return false;

			CurrentFrame++;
			MovementSystem.ProcessShift(false, 1 - ClassManager.GameData.SubFrame, ClassManager.GameData);
			FallEvent(null, 1 - ClassManager.GameData.SubFrame);

			if (events == null)
				return true;

			ExcuteWaitingFrames();



			if (ClassManager.GameData.Options.GravityIncrease > 0 &&
				CurrentFrame > ClassManager.GameData.Options.GravityMargin)
				ClassManager.GameData.Gravity += ClassManager.GameData.Options.GravityIncrease / 60;

			if (ClassManager.GameData.Options.GarbageIncrease > 0 &&
				CurrentFrame > ClassManager.GameData.Options.GarbageMargin)
				ClassManager.GameData.Options.GarbageMultiplier += ClassManager.GameData.Options.GarbageIncrease / 60;

			if (ClassManager.GameData.Options.GarbageCapIncrease > 0)
				ClassManager.GameData.Options.GarbageCap += ClassManager.GameData.Options.GarbageCapIncrease / 60;



			return true;
		}

		/// <summary>
		/// Update game during events[CurrentIndex].frame == CurrentFrame 
		/// </summary>
		/// <param name="events"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private bool PullEvent(IReadOnlyList<Event> events)
		{
			while (true)
			{
				if (events[CurrentIndex].frame == CurrentFrame)
				{

					switch (events[CurrentIndex].type)
					{
						case "start":
							break;

						case "full":
							ClassManager.GameData.Falling.Init(null, false, NextGenerateMode);
							break;

						case "keydown":
						case "keyup":
							var inputEvent = JsonSerializer.Deserialize<EventKeyInput>(events[CurrentIndex].data.ToString());

							if (inputEvent == null)
								throw new Exception("inputEvent is null.");

							if (events[CurrentIndex].type == "keydown")
							{
								PressedKeyList.Add(inputEvent.key);
							}
							else
							{
								PressedKeyList.Remove(inputEvent.key);
							}

							KeyInput(events[CurrentIndex].type, inputEvent);

							break;


						case "targets":
							var targets = JsonSerializer.Deserialize<EventTargets>(events[CurrentIndex].data.ToString());
							for (int i = 0; i < targets.data.Count; i++)
								targets.data[i] = targets.data[i].Substring(0, 24);

							GameData.Targets = targets.data;

							break;

						case "ige":
							var igeStr = events[CurrentIndex].data.ToString();
							var garbageEvent = JsonSerializer.Deserialize<EventIge>(igeStr);


							if (events[CurrentIndex].id != null)
								Console.WriteLine("予期せぬEvent状態をしています。正常にリプレイが再生されない可能性があります。");


							if (GarbageIDs.Contains((int)garbageEvent.id))
								break;

							GarbageIDs.Add((int)garbageEvent.id);

							if (garbageEvent.data.type == "attack")
							{
								IncomingAttack(new GarbageData()
								{
									type = "garbage",
									amt = garbageEvent.data.lines,
									column = garbageEvent.data.column,
									x = -1,
									y = -1,
								}, garbageEvent.data.sender, garbageEvent.data.cid.ToString());
							}

							if (garbageEvent.data.type == "interaction")
							{
								switch (garbageEvent.data.data.type)
								{
									case "garbage":
										string? idValue;
										if (GameData.Options.ClipListenIDs)
											idValue = garbageEvent.data.sender_id != null ? garbageEvent.data.sender_id.Substring(0, 24) : null;
										else
											idValue = garbageEvent.data.sender_id;
										StartingAttack(garbageEvent.data.data, garbageEvent.data.sender, idValue, garbageEvent.data.cid);

										break;
								}
							}

							if (garbageEvent.data.type == "interaction_confirm")
							{
								switch (garbageEvent.data.data.type)
								{
									case "garbage":
										string idValue;
										if (GameData.Options.ClipListenIDs)
											idValue = garbageEvent.data.sender_id != null ? garbageEvent.data.sender_id.Substring(0, 24) : null;
										else
											idValue = garbageEvent.data.sender_id;

										IncomingAttack(garbageEvent.data.data, garbageEvent.data.sender, idValue, garbageEvent.data.cid);
										break;
								}
							}

							if (garbageEvent.data.type == "kev")
							{

							}

							break;

						case "end":
							return false;

						default:
							throw new Exception("unknown event type: " + events[CurrentIndex].type);
					}

					CurrentIndex++;
				}
				else break;
			}


			return true;
		}

		private void StartingAttack(GarbageData data, string sender, string? id1, int? id2)
		{
			InsertDamage(data, sender, id1, id2);
		}

		private void IncomingAttack(GarbageData data, string? sender = null, string? id1 = null, int? id2 = null)
		{


			if (GameData.Options.Passthrough == "consistent")
			{
				data.amt = GetAmt(data, id1);
			}
			else
			{
				if (id2 != null)
					data.amt = GetImpendingGarbageLineAtID(id2);

				if (data.amt <= 0)
					return;


			}



			WaitFrames(GameData.Options.GarbageSpeed, "incoming-attack-hit", new IgeData()
			{
				data = data,
				sender = sender,
				cid = id2
			});


			int GetImpendingGarbageLineAtID(int? garbageID)
			{
				foreach (var garbage in GameData.ImpendingDamages)
				{
					if (garbage.cid == garbageID)
						return garbage.lines;
				}

				return 0;
			}
		}

		private void WaitFrames(int waitFrame, string kind, object data)
		{
			GameData.WaitingFrames.Add(new WaitingFrameData()
			{
				target = CurrentFrame + waitFrame,
				type = kind,
				AdditionalData = data
			});
		}

		private void ExcuteWaitingFrames()
		{
			for (int waitingFrameIndex = GameData.WaitingFrames.Count - 1; waitingFrameIndex >= 0; waitingFrameIndex--)
			{
				if (CurrentFrame == GameData.WaitingFrames[waitingFrameIndex].target)
				{
					ExcuteWaitingFrame(GameData.WaitingFrames[waitingFrameIndex]);
					GameData.WaitingFrames.RemoveAt(waitingFrameIndex);
				}


			}
		}

		private void ExcuteWaitingFrame(WaitingFrameData data)
		{
			if (data.type == "incoming-attack-hit")
			{
				var igedata = (IgeData)data.AdditionalData;
				IncomingAttackHit(igedata.data, igedata.sender, igedata.cid);
			}
			else if (data.type == "outgoing-attack-hit")
			{
				// OutgoingAttackHit();
			}
			else if (data.type == "are")
			{
				throw new NotImplementedException();
			}
			else if (data.type == "freeze-counters")
			{
				throw new NotImplementedException();
			}
			else if (data.type == "revive-from-stock-loss")
			{
				throw new NotImplementedException();
			}
			else
			{
				throw new Exception("unknown type");
			}
		}


		private void IncomingAttackHit(GarbageData data, string sender, int? cid)
		{
			GameData.NotYetReceivedAttack--;

			if (cid != null)
			{
				//ActiveDamage Function
				foreach (var garbage in GameData.ImpendingDamages)
				{
					if (garbage.cid == cid)
						garbage.active = true;
				}
			}
			else
			{
				InsertDamage(data, sender);
			}
		}


		private void InsertDamage(GarbageData data, string sender, string? id1 = null, int? id2 = null)
		{
			var amt_value = data.amt;

			if (GameData.Options.Passthrough == "zero")
			{
				amt_value = GetAmt(data, id1);
			}

			if (!(amt_value <= 0))
			{
				GameData.ImpendingDamages.Add(new IgeData()
				{
					id = ++GameData.GarbageID,
					type = data.type,
					active = id2 == null,
					lines = amt_value,
					column = data.column,
					data = data,
					sender = sender,
					cid = id2
				});
			}


		}

		private int GetAmt(GarbageData data, string? id = null)
		{
			if (!GameData.GarbageActnowledgements.Incoming.ContainsKey(id))
				GameData.GarbageActnowledgements.Incoming.Add(id, data.iid);
			else
				GameData.GarbageActnowledgements.Incoming[id] = data.iid;

			var amtValue = data.amt;

			if (GameData.GarbageActnowledgements.Outgoing.ContainsKey(id))
			{
				var newArray = new List<GarbageData?>();

				foreach (var outgoing in GameData.GarbageActnowledgements.Outgoing[id])
				{
					if (outgoing.iid <= data.ackiid)
						continue;
					var minValue = Math.Min(outgoing.amt, amtValue);
					outgoing.amt -= minValue;
					amtValue -= minValue;



					if (outgoing.amt > 0)
						newArray.Add(outgoing);
				}

				GameData.GarbageActnowledgements.Outgoing[id] = newArray;
			}

			return amtValue;
		}



		private void FallEvent(int? value, double subFrameDiff)
		{
			if (ClassManager.GameData.Falling.SafeLock > 0)
				ClassManager.GameData.Falling.SafeLock--;

			if (ClassManager.GameData.Falling.Sleep || ClassManager.GameData.Falling.DeepSleep)
				return;

			var subframeGravity = ClassManager.GameData.Gravity * subFrameDiff;

			if (ClassManager.GameData.SoftDrop)
			{
				if (ClassManager.GameData.Handling.SDF == (ClassManager.GameData.Options.Version >= 15 ? 41 : 21))
					subframeGravity = (ClassManager.GameData.Options.Version >= 15 ? 400 : 20) * subFrameDiff;
				else
				{
					subframeGravity *= ClassManager.GameData.Handling.SDF;
					subframeGravity = Math.Max(subframeGravity, ClassManager.GameData.Options.Version >= 13 ?
						0.05 * ClassManager.GameData.Handling.SDF : 0.42);
				}
			}

			if (value != null)
				subframeGravity = (int)value;

			if (!ClassManager.GameData.Options.InfiniteMovement &&
				ClassManager.GameData.Falling.LockResets >= (int)ClassManager.GameData.Options.LockResets &&
				!JudgeSystem.IsLegalAtPos(ClassManager.GameData.Falling.Type, ClassManager.GameData.Falling.X,
				ClassManager.GameData.Falling.Y + 1, ClassManager.GameData.Falling.R, ClassManager.GameData.Board))
			{
				subframeGravity = 20;
				ClassManager.GameData.Falling.ForceLock = true;
			}


			if (!ClassManager.GameData.Options.InfiniteMovement &&
				ClassManager.GameData.FallingRotations > (int)ClassManager.GameData.Options.LockResets + 15)
			{
				subframeGravity += 0.5 * subFrameDiff *
					(ClassManager.GameData.FallingRotations - ((int)ClassManager.GameData.Options.LockResets + 15));
			}

			double constSubFrameGravity = subframeGravity;

			for (; subframeGravity > 0;)
			{
				var ceiledValue = Math.Ceiling(ClassManager.GameData.Falling.Y);
				if (!HardDrop(Math.Min(1, subframeGravity), constSubFrameGravity))
				{
					if (value != null)
						ClassManager.GameData.Falling.ForceLock = true;
					Locking(value != 0 && value != null, subFrameDiff);
					break;
				}

				subframeGravity -= Math.Min(1, subframeGravity);
				if (ceiledValue != Math.Ceiling(ClassManager.GameData.Falling.Y))
				{
					ClassManager.GameData.Falling.Last = "fall";
					if (ClassManager.GameData.SoftDrop)
					{
						//ScoreAdd

					}
				}
			}
		}

		private bool HardDrop(double value, double value2)
		{
			var yPos1 = Math.Floor(Math.Pow(10, 13) * ClassManager.GameData.Falling.Y) /
				Math.Pow(10, 13) + value;

			if (yPos1 % 1 == 0)
				yPos1 += 0.00001;

			var yPos2 = Math.Floor(Math.Pow(10, 13) * ClassManager.GameData.Falling.Y) / Math.Pow(10, 13) + 1;
			if (yPos2 % 1 == 0)
				yPos2 -= 0.00002;

			if (JudgeSystem.IsLegalAtPos(ClassManager.GameData.Falling.Type, ClassManager.GameData.Falling.X, yPos1, ClassManager.GameData.Falling.R, ClassManager.GameData.Board) &&
				JudgeSystem.IsLegalAtPos(ClassManager.GameData.Falling.Type, ClassManager.GameData.Falling.X, yPos2, ClassManager.GameData.Falling.R, ClassManager.GameData.Board))
			{
				var highestY = ClassManager.GameData.Falling.HighestY;
				yPos2 = ClassManager.GameData.Falling.Y;

				ClassManager.GameData.Falling.Y = yPos1;
				ClassManager.GameData.Falling.HighestY = Math.Ceiling(Math.Max(ClassManager.GameData.Falling.HighestY, yPos1));
				ClassManager.GameData.Falling.Floored = false;
				if (Math.Ceiling(yPos1) != Math.Ceiling(yPos2))
				{

				}

				if (yPos1 > highestY || ClassManager.GameData.Options.InfiniteMovement)
					ClassManager.GameData.Falling.LockResets = 0;
				ClassManager.GameData.FallingRotations = 0;

				if (value2 >= int.MaxValue)
				{
					//スコア足す
				}

				return true;
			}

			return false;
		}

		private void Lock(bool sValue, bool emptyDrop = false)
		{
			ClassManager.GameData.Falling.Sleep = true;
			//ミノを盤面に適用
			if (!emptyDrop)
			{
				foreach (var pos in ConstData.TETRIMINOS_SHAPES[ClassManager.GameData.Falling.Type][ClassManager.GameData.Falling.R])
				{
					ClassManager.GameData.Board[(int)((pos.x + ClassManager.GameData.Falling.X - ConstData.TETRIMINO_DIFFS[ClassManager.GameData.Falling.Type].x) +
						(int)(pos.y + ClassManager.GameData.Falling.Y - ConstData.TETRIMINO_DIFFS[ClassManager.GameData.Falling.Type].y) * 10)] = ClassManager.GameData.Falling.Type;
				}

				if (!sValue && ClassManager.GameData.Handling.SafeLock > 0)
					ClassManager.GameData.Falling.SafeLock = 7;

			}

			var isTspin = JudgeSystem.IsTspin(ClassManager.GameData);
			var clearedLineCount = ClearLines();

			var announceLine = GarbageSystem.GetAttackPower(clearedLineCount, isTspin, ClassManager);
			JudgeSystem.IsBoardEmpty(ClassManager.GameData);

			if (!announceLine)
				TakeAllDamage();


			ClassManager.GameData.Falling.Init(null, false, NextGenerateMode);

			OnPiecePlaced?.Invoke(this, EventArgs.Empty);

		}

		internal void CallOnPieceCreated()
		{
			OnPieceCreated?.Invoke(this, EventArgs.Empty);
		}



		public void UserInput(Action action)
		{
			switch (action)
			{
				case Action.MoveRight:
					if (JudgeSystem.IsLegalAtPos(ClassManager.GameData.Falling.Type, ClassManager.GameData.Falling.X + 1, ClassManager.GameData.Falling.Y, ClassManager.GameData.Falling.R, ClassManager.GameData.Board))
						ClassManager.GameData.Falling.X++;
					break;

				case Action.MoveLeft:
					if (JudgeSystem.IsLegalAtPos(ClassManager.GameData.Falling.Type, ClassManager.GameData.Falling.X - 1, ClassManager.GameData.Falling.Y, ClassManager.GameData.Falling.R, ClassManager.GameData.Board))
						ClassManager.GameData.Falling.X--;
					break;

				case Action.RotateLeft:
					var e = ClassManager.GameData.Falling.R - 1;
					if (e < 0)
						e = 3;
					RotateSystem.RotatePiece(e, ClassManager.GameData);
					break;

				case Action.RotateRight:
					e = ClassManager.GameData.Falling.R + 1;
					if (e > 3)
						e = 0;
					RotateSystem.RotatePiece(e, ClassManager.GameData);
					break;

				case Action.Rotate180:
					e = ClassManager.GameData.Falling.R + 2;
					if (e > 3)
						e -= 4;
					RotateSystem.RotatePiece(e, ClassManager.GameData);
					break;

				case Action.Hold:
					SwapHold();
					break;

				case Action.Harddrop:
					FallEvent(int.MaxValue, 1);
					break;

				case Action.QuickSoftdrop:
					while (true)
					{
						if (JudgeSystem.IsLegalAtPos(ClassManager.GameData.Falling.Type, ClassManager.GameData.Falling.X, ClassManager.GameData.Falling.Y + 1, ClassManager.GameData.Falling.R, ClassManager.GameData.Board))
							ClassManager.GameData.Falling.Y++;
						else
							break;
					}
					break;

				case Action.MoveUp:
					if (JudgeSystem.IsLegalAtPos(ClassManager.GameData.Falling.Type, ClassManager.GameData.Falling.X, ClassManager.GameData.Falling.Y - 1, ClassManager.GameData.Falling.R, ClassManager.GameData.Board))
						ClassManager.GameData.Falling.Y--;
					break;

				case Action.MoveDown:
					if (JudgeSystem.IsLegalAtPos(ClassManager.GameData.Falling.Type, ClassManager.GameData.Falling.X, ClassManager.GameData.Falling.Y + 1, ClassManager.GameData.Falling.R, ClassManager.GameData.Board))
						ClassManager.GameData.Falling.Y++;
					break;

				default: throw new Exception();
			}

		}

		/// <summary>
		/// This is for DEBUG
		/// </summary>
		/// <param name="kind"></param>
		/// <returns></returns>
		public object GetData(string kind)
		{
			switch (kind)
			{
				case "safelock":

					break;
			}

			if (kind == "safelock")
				return ClassManager.GameData.Falling.SafeLock;

			return null;
		}

		private int ClearLines()
		{
			List<int> list = new List<int>();

			for (int y = FIELD_HEIGHT - 1; y >= 0; y--)
			{
				bool flag = true;
				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (ClassManager.GameData.Board[x + y * 10] == (int)MinoKind.Empty)
					{
						flag = false;
						break;
					}
				}

				if (flag)
					list.Add(y);
			}

			list.Reverse();
			foreach (var value in list)
			{
				DownLine(value, ClassManager.GameData.Board);
			}

			return list.Count;

		}

		public static Vector2[] GetMinoPos(int type, int x, int y, int r)
		{
			var positions = (Vector2[])ConstData.TETRIMINOS_SHAPES[type][r].Clone();
			var diff = ConstData.TETRIMINO_DIFFS[type];

			for (int i = 0; i < positions.Length; i++)
			{
				positions[i].x += x - diff.x;
				positions[i].y += y - diff.y;
			}

			return positions;
		}

		public int GetFallestPosDiff()
		{

			return GetFallestPosDiff(ClassManager.GameData.Falling.Type, ClassManager.GameData.Falling.X, (int)ClassManager.GameData.Falling.Y, ClassManager.GameData.Falling.R);

		}

		public int GetFallestPosDiff(int type, int x, int y, int r)
		{
			if (type == -1)
				return 0;

			int testY = 1;
			while (true)
			{
				if (JudgeSystem.IsLegalAtPos(type, x, y + testY,
				   r, ClassManager.GameData.Board))
					testY++;
				else
				{
					testY--;
					break;
				}
			}

			return testY;
		}


		/// <summary>
		/// 消去したラインを下げる
		/// </summary>
		/// <param name="value">y座標</param>
		/// <param name="field">盤面</param>
		private void DownLine(int value, int[] field)
		{
			for (int y = value; y >= 0; y--)
			{
				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (y - 1 == -1)
						field[x + y * 10] = (int)(MinoKind.Empty);
					else
						field[x + y * 10] = field[x + (y - 1) * 10];
				}
			}
		}

		/// <summary>
		/// Place piece if threshold is over.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="subframe"></param>
		private void Locking(bool value = false, double subframe = 1)
		{
			if (ClassManager.GameData.Options.Version >= 15)
				ClassManager.GameData.Falling.Locking += subframe;
			else
				ClassManager.GameData.Falling.Locking += 1;

			if (!ClassManager.GameData.Falling.Floored)
				ClassManager.GameData.Falling.Floored = true;



			if (ClassManager.GameData.Falling.Locking > ClassManager.GameData.Options.LockTime ||
				ClassManager.GameData.Falling.ForceLock ||
				ClassManager.GameData.Falling.LockResets > ClassManager.GameData.Options.LockResets &&
				!ClassManager.GameData.Options.InfiniteMovement)
			{
				Lock(value);
			}
		}

		internal int RefreshNext(List<int> next, bool noszo)
		{
			GeneratedRngCount++;
			var value = next[0];

			for (int i = 0; i < next.Count - 1; i++)
			{
				next[i] = next[i + 1];
			}

			if (ClassManager.GameData.NextBag.Count == 0)
			{
				var array = (int[])ConstData.MINOTYPES.Clone();
				Rng.ShuffleArray(array);
				ClassManager.GameData.NextBag.AddRange(array);
			}

			if (noszo)
			{
				while (true)
				{
					if (ClassManager.GameData.NextBag[0] == (int)MinoKind.S ||
						 ClassManager.GameData.NextBag[0] == (int)MinoKind.Z ||
						ClassManager.GameData.NextBag[0] == (int)MinoKind.O)
					{
						var temp = ClassManager.GameData.NextBag[0];
						for (int i = 0; i < ClassManager.GameData.NextBag.Count - 1; i++)
						{
							ClassManager.GameData.NextBag[i] = ClassManager.GameData.NextBag[i + 1];
						}
						ClassManager.GameData.NextBag[^1] = temp;
					}
					else
						break;
				}

			}


			next[^1] = ClassManager.GameData.NextBag[0];
			ClassManager.GameData.NextBag.RemoveAt(0);
			return value;
		}

		private void SwapHold()
		{
			var tempCurrentType = ClassManager.GameData.Falling.Type;
			ClassManager.GameData.Falling.Init(ClassManager.GameData.Hold, true, NextGenerateMode);
			ClassManager.GameData.Hold = tempCurrentType;
		}


		private void TakeAllDamage()
		{
			if (!GameData.Options.HasGarbage)
				return;

			var ABoolValue = true;
			int maxReceiveGarbage = false ? 400 : (int)Math.Min(GameData.Options.GarbageCapMax, GameData.Options.GarbageCap);
			var oValue = false;
			var iArray = new List<IgeData>();

			for (var impendingIndex = GameData.ImpendingDamages.Count - 1; impendingIndex >= 0; impendingIndex--)
			{
				if (!GameData.ImpendingDamages[impendingIndex].active)
				{
					iArray.Insert(0, GameData.ImpendingDamages[impendingIndex]);
					GameData.ImpendingDamages.RemoveAt(impendingIndex);
				}
			}

			for (var i = 0; i < maxReceiveGarbage && GameData.ImpendingDamages.Count != 0; i++)
			{
				GameData.ImpendingDamages[0].lines--;
				oValue = true;
				bool rValue;
				if (GameData.ImpendingDamages[0].lines <= 0)
					rValue = true;
				else
					rValue = i >= maxReceiveGarbage - 1;

				if (!GarbageSystem.PushGarbageLine(GameData.ImpendingDamages[0].column, ClassManager.GameData, false,
						(ABoolValue ? 64 : 0) | (rValue ? 4 : 0)))
					break;

				ABoolValue = false;

				if (!(GameData.ImpendingDamages[0].lines > 0))
				{
					GameData.ImpendingDamages.RemoveAt(0);
					ABoolValue = true;
				}

			}

			GameData.ImpendingDamages.AddRange(iArray.ToArray());

		}






	}
}
using System.Diagnostics;
using System.Dynamic;
using System.Numerics;
using System.Security.Claims;
using System.Text.Json;
using TetrReplayLoader;
using TetrReplayLoader.JsonClass;
using TetrReplayLoader.JsonClass.Event;

namespace TetrioVirtualEnvironment
{
	public class Environment
	{
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
		/// <summary>
		/// GarbageList
		/// </summary>
		public List<Garbage> Garbages { get; private set; }
		/// <summary>
		/// GarbageList for Garbage.GarbageKind.Attack
		/// attak event maybe caused by lag?
		/// </summary>
		public List<Garbage> GarbagesInterrupt { get; private set; }
		public GameData GameData => _gameData;
		public Stats Stats { get; private set; }
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
		/// Kickset of SRS+
		/// </summary>
		private Dictionary<string, Vector2[]> KICKSET_SRSPLUS { get; }
		/// <summary>
		/// Kickset of SRS+ I-piece
		/// </summary>
		private Dictionary<string, Vector2[]> KICKSET_SRSPLUSI { get; }

		private GameData _gameData;
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
			dataForInitialize ??= new DataForInitialize();


			Username = username;
			NextGenerateMode = envMode;
			EventFull = envData;
			DataForInitialize = dataForInitialize;
			NextSkipCount = nextSkipCount;

			ResetGame(envData, envMode, dataForInitialize, nextSkipCount);

			var tempkickset = new Dictionary<string, Vector2[]>();
			var tempkicksetI = new Dictionary<string, Vector2[]>();

			#region KickSet Init
			tempkickset.Add("01", new[] { new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, 2), new Vector2(-1, 2), });
			tempkickset.Add("10", new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(1, -2), });
			tempkickset.Add("12", new[] { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, -2), new Vector2(1, -2), });
			tempkickset.Add("21", new[] { new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, 2), new Vector2(-1, 2), });
			tempkickset.Add("23", new[] { new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, 2), new Vector2(1, 2), });
			tempkickset.Add("32", new[] { new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -2), new Vector2(-1, -2), });
			tempkickset.Add("30", new[] { new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, -2), new Vector2(-1, -2), });
			tempkickset.Add("03", new[] { new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, 2), new Vector2(1, 2), });
			tempkickset.Add("02", new[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(1, 0), new Vector2(-1, 0), });
			tempkickset.Add("13", new[] { new Vector2(1, 0), new Vector2(1, -2), new Vector2(1, -1), new Vector2(0, -2), new Vector2(0, -1), });
			tempkickset.Add("20", new[] { new Vector2(0, 1), new Vector2(-1, 1), new Vector2(1, 1), new Vector2(-1, 0), new Vector2(1, 0), });
			tempkickset.Add("31", new[] { new Vector2(-1, 0), new Vector2(-1, -2), new Vector2(-1, -1), new Vector2(0, -2), new Vector2(0, -1), });
			KICKSET_SRSPLUS = tempkickset;

			tempkicksetI.Add("01", new[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(-2, 1), new Vector2(1, -2), });
			tempkicksetI.Add("10", new[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(-1, 2), new Vector2(2, -1), });
			tempkicksetI.Add("12", new[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(-1, -2), new Vector2(2, 1), });
			tempkicksetI.Add("21", new[] { new Vector2(-2, 0), new Vector2(1, 0), new Vector2(-2, -1), new Vector2(1, 2), });
			tempkicksetI.Add("23", new[] { new Vector2(2, 0), new Vector2(-1, 0), new Vector2(2, -1), new Vector2(-1, 2), });
			tempkicksetI.Add("32", new[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(1, -2), new Vector2(-2, 1), });
			tempkicksetI.Add("30", new[] { new Vector2(1, 0), new Vector2(-2, 0), new Vector2(1, 2), new Vector2(-2, -1), });
			tempkicksetI.Add("03", new[] { new Vector2(-1, 0), new Vector2(2, 0), new Vector2(2, 1), new Vector2(-1, -2), });
			tempkicksetI.Add("02", new[] { new Vector2(0, -1) });
			tempkicksetI.Add("13", new[] { new Vector2(1, 0) });
			tempkicksetI.Add("20", new[] { new Vector2(0, 1) });
			tempkicksetI.Add("31", new[] { new Vector2(-1, 0) });
			KICKSET_SRSPLUSI = tempkicksetI;
			#endregion

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
			Garbages = new List<Garbage>();
			GarbagesInterrupt = new List<Garbage>();
			GarbageIDs = new List<int>();
			CurrentFrame = 0;
			CurrentIndex = 0;
			Stats = new Stats();

			new GameData(out _gameData, envMode, envData, this, nextSkipCount, dataForInitialize);
		}

		//TODO: remove this
		/// <summary>
		/// value%10 GarbageKind
		/// value/10%10 GarbagePos
		/// value/100 Amount of Garbage
		/// </summary>
		/// <returns></returns>
		public int[] GetGarbagesArray()
		{
			List<int> garbagesArray = new List<int>();

			foreach (var value in Garbages)
			{
				var temp = 1;

				if (value.State == Garbage.GarbageKind.Ready)
					temp *= 1;
				else
					temp *= 0;

				temp += value.PosX * 10;

				temp += value.Power * 100;


				garbagesArray.Add(temp);
			}

			foreach (var value in GarbagesInterrupt)
			{
				var temp = 0;

				temp += value.PosX * 10;
				temp += value.Power * 100;
				garbagesArray.Add(temp);
			}

			return garbagesArray.ToArray();
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

		/// <summary>
		/// Judge position is in the field and selected position is empty.
		/// </summary>
		/// <param name="x">absolute position</param>
		/// <param name="y">absolute position</param>
		/// <param name="field"></param>
		/// <returns>Is empty</returns>
		private static bool IsEmptyPos(int x, int y, IReadOnlyList<int> field)
		{
			if (!(x is >= 0 and < FIELD_WIDTH &&
				  y is >= 0 and < FIELD_HEIGHT))
				return false;

			return field[x + y * 10] == (int)MinoKind.Empty;
		}

		/// <summary>
		/// Check tetromino is in the field and not being overwraped.
		/// remove this
		/// </summary>
		/// <param name="type"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public static bool IsLegalField(int type, int x, double y, int rotation)
		{
			var positions = ConstData.TETRIMINOS_SHAPES[type][rotation];
			var diff = ConstData.TETRIMINO_DIFFS[type];

			foreach (var position in positions)
			{
				if (!(position.x + x - diff.x >= 0 && position.x + x - diff.x < FIELD_WIDTH &&
				  position.y + y - diff.y >= 0 && position.y + y - diff.y < FIELD_HEIGHT))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Check x,y is in the field.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool IsLegalField(int x, double y)
		{
			return x is >= 0 and < FIELD_WIDTH &&
				   y is >= 0 and < FIELD_HEIGHT;
		}

		/// <summary>
		/// Check tetromino is in the field and not being overwraped.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="rotation"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		public static bool IsLegalAtPos(int type, int x, double y, int rotation, IReadOnlyList<int> field)
		{
			var positions = ConstData.TETRIMINOS_SHAPES[type][rotation];
			var diff = ConstData.TETRIMINO_DIFFS[type];

			foreach (var position in positions)
			{
				if (!(position.x + x - diff.x >= 0 && position.x + x - diff.x < FIELD_WIDTH &&
				  position.y + y - diff.y >= 0 && position.y + y - diff.y < FIELD_HEIGHT &&
					 field[(int)position.x + x - (int)diff.x + (int)(position.y + y - (int)diff.y) * 10] == (int)MinoKind.Empty))
					return false;
			}

			return true;
		}



		public void KeyInput(string type, EventKeyInput @event)
		{
			if (type == "keydown")
			{
				if (@event.subframe > _gameData.SubFrame)
				{
					ProcessShift(false, @event.subframe - _gameData.SubFrame);
					FallEvent(null, @event.subframe - _gameData.SubFrame);
					_gameData.SubFrame = @event.subframe;
				}

				if (@event.key == "moveLeft")
				{
					_gameData.LShift = true;
					_gameData.LastShift = -1;
					_gameData.LDas = @event.hoisted ?
						_gameData.Handling.DAS - _gameData.Handling.DCD : 0;
					if (_gameData.Options.Version >= 12)
						_gameData.LDasIter = _gameData.Handling.ARR;

					ProcessLShift(true, _gameData.Options.Version >= 15 ? 0 : 1);
					return;
				}

				if (@event.key == "moveRight")
				{
					_gameData.RShift = true;
					_gameData.LastShift = 1;
					_gameData.RDas = @event.hoisted ? _gameData.Handling.DAS - _gameData.Handling.DCD : 0;
					if (_gameData.Options.Version >= 12)
						_gameData.RDasIter = _gameData.Handling.ARR;

					ProcessRShift(true, _gameData.Options.Version >= 15 ? 0 : 1);
					return;
				}

				if (@event.key == "softDrop")
				{
					_gameData.SoftDrop = true;
					return;
				}

				if (_gameData.Falling.Sleep || _gameData.Falling.DeepSleep)
				{
					throw new NotImplementedException();
				}
				else
				{
					if (@event.key == "rotateCCW")
					{
						var e = _gameData.Falling.R - 1;
						if (e < 0)
							e = 3;
						RotatePiece(e);
					}

					if (@event.key == "rotateCW")
					{
						var e = _gameData.Falling.R + 1;
						if (e > 3)
							e = 0;
						RotatePiece(e);
					}

					if (@event.key == "rotate180" && _gameData.Options.Allow180)
					{
						var e = _gameData.Falling.R + 2;
						if (e > 3)
							e -= 4;
						RotatePiece(e);
					}
					if (@event.key == "hardDrop" && _gameData.Options.AllowHardDrop &&
						_gameData.Falling.SafeLock == 0)
					{
						FallEvent(int.MaxValue, 1);
					}

					if (@event.key == "hold")
					{
						if (!_gameData.HoldLocked || InfinityHold)
						{
							if ((_gameData.Options.DisplayHold == null ||
							(bool)_gameData.Options.DisplayHold))
								SwapHold();
						}
					}
				}

			}
			else if (type == "keyup")
			{
				if (@event.subframe > _gameData.SubFrame)
				{
					ProcessShift(false, @event.subframe - _gameData.SubFrame);
					FallEvent(null, @event.subframe - _gameData.SubFrame);
					_gameData.SubFrame = @event.subframe;
				}

				if (@event.key == "moveLeft")
				{
					_gameData.LShift = false;
					_gameData.LDas = 0;

					if (_gameData.Handling.Cancel)
					{
						_gameData.RDas = 0;
						_gameData.RDasIter = _gameData.Handling.ARR;
					}

					return;
				}

				if (@event.key == "moveRight")
				{
					_gameData.RShift = false;
					_gameData.RDas = 0;

					if (_gameData.Handling.Cancel)
					{
						_gameData.LDas = 0;
						_gameData.LDasIter = _gameData.Handling.ARR;
					}

					return;
				}

				if (@event.key == "softDrop")
					_gameData.SoftDrop = false;


			}
		}

		private void RotatePiece(int rotation)
		{
			var nowminoRotation = _gameData.Falling.R;
			var nowminoNewminoRotation = nowminoRotation.ToString() + rotation.ToString();
			var o = 0;
			var i = "none";

			if (rotation < nowminoRotation)
			{
				o = 1;
				i = "right";
			}
			else
			{
				o = -1;
				i = "left";
			}

			if (rotation == 0 && nowminoRotation == 3)
			{
				o = 1;
				i = "right";
			}

			if (rotation == 3 && nowminoRotation == 3)
			{
				o = -1;
				i = "left";
			}

			if (rotation == 2 && nowminoRotation == 0)
				i = "vertical";
			if (rotation == 0 && nowminoRotation == 2)
				i = "vertical";
			if (rotation == 3 && nowminoRotation == 1)
				i = "horizontal";
			if (rotation == 1 && nowminoRotation == 3)
				i = "horizontal";

			if (IsLegalAtPos(_gameData.Falling.Type,
				_gameData.Falling.X - _gameData.Falling.Aox,
				_gameData.Falling.Y - _gameData.Falling.Aoy, rotation,
				_gameData.Board))
			{
				_gameData.Falling.X -= _gameData.Falling.Aox;
				_gameData.Falling.Y -= _gameData.Falling.Aoy;
				_gameData.Falling.Aox = 0;
				_gameData.Falling.Aoy = 0;
				_gameData.Falling.R = rotation;
				_gameData.Falling.Last = "rotate";
				_gameData.Falling.LastRotation = i;
				_gameData.Falling.LastKick = 0;
				_gameData.Falling.SpinType = IsTspin();
				_gameData.FallingRotations++;
				_gameData.TotalRotations++;

				if (_gameData.Falling.Clamped && _gameData.Handling.DCD > 0)
				{
					_gameData.LDas = Math.Min(_gameData.LDas, _gameData.Handling.DAS - _gameData.Handling.DCD);
					_gameData.LDasIter = _gameData.Handling.ARR;
					_gameData.RDas = Math.Min(_gameData.RDas, _gameData.Handling.DAS - _gameData.Handling.DCD);
					_gameData.RDasIter = _gameData.Handling.ARR;
				}

				if (++_gameData.Falling.LockResets < 15 || _gameData.Options.InfiniteMovement)
					_gameData.Falling.Locking = 0;


				//落下ミノ更新フラグ true
				return;
			}

			if (_gameData.Falling.Type == (int)MinoKind.O)
				return;

			var kicktable = KICKSET_SRSPLUS[nowminoNewminoRotation];

			if (_gameData.Falling.Type == (int)MinoKind.I)
				kicktable = KICKSET_SRSPLUSI[nowminoNewminoRotation];

			for (var kicktableIndex = 0; kicktableIndex < kicktable.Length; kicktableIndex++)
			{
				var kicktableTest = kicktable[kicktableIndex];
				var i2 = (int)(_gameData.Falling.Y) + 0.1 +
					kicktableTest.y - _gameData.Falling.Aoy;


				if (!_gameData.Options.InfiniteMovement &&
					_gameData.TotalRotations > (int)_gameData.Options.LockResets + 15)
				{
					i2 = _gameData.Falling.Y + kicktableTest.y - _gameData.Falling.Aoy;
				}

				if (IsLegalAtPos(_gameData.Falling.Type,
					_gameData.Falling.X + (int)kicktableTest.x - _gameData.Falling.Aox,
					i2, rotation, _gameData.Board))
				{

					_gameData.Falling.X += (int)kicktableTest.x - _gameData.Falling.Aox;
					_gameData.Falling.Y = i2;
					_gameData.Falling.Aox = 0;
					_gameData.Falling.Aoy = 0;
					_gameData.Falling.R = rotation;
					_gameData.Falling.SpinType = IsTspin();
					_gameData.Falling.LastKick = kicktableIndex + 1;
					_gameData.Falling.Last = "rotate";
					_gameData.FallingRotations++;
					_gameData.TotalRotations++;

					if (_gameData.Falling.Clamped && _gameData.Handling.DCD > 0)
					{
						_gameData.LDas = Math.Min(_gameData.LDas, _gameData.Handling.DAS - _gameData.Handling.DCD);
						_gameData.LDasIter = _gameData.Handling.ARR;
						_gameData.RDas = Math.Min(_gameData.RDas, _gameData.Handling.DAS - _gameData.Handling.DCD);
						_gameData.RDasIter = _gameData.Handling.ARR;
					}

					if (++_gameData.Falling.LockResets < 15 || _gameData.Options.InfiniteMovement)
						_gameData.Falling.Locking = 0;


					return;
				}
			}
		}


		/// <summary>
		/// Right Left shift
		/// </summary>
		/// <param name="value"></param>
		/// <param name="subFrameDiff"></param>
		private void ProcessShift(bool value, double subFrameDiff)
		{
			ProcessLShift(value, subFrameDiff);
			ProcessRShift(value, subFrameDiff);
		}

		/// <summary>
		/// Update game. Don't call if environment is managed by Replay class.
		/// </summary>
		/// <param name="events"></param>
		/// <returns></returns>
		public bool Update(IReadOnlyList<Event>? events = null)
		{
			_gameData.SubFrame = 0;

			if (events != null && !PullEvent(events))
				return false;

			CurrentFrame++;
			ProcessShift(false, 1 - _gameData.SubFrame);
			FallEvent(null, 1 - _gameData.SubFrame);

			if (events == null)
				return true;

			ExcuteWaitingFrames();



			if (_gameData.Options.GravityIncrease > 0 &&
				CurrentFrame > _gameData.Options.GravityMargin)
				_gameData.Gravity += _gameData.Options.GravityIncrease / 60;

			if (_gameData.Options.GarbageIncrease > 0 &&
				CurrentFrame > _gameData.Options.GarbageMargin)
				_gameData.Options.GarbageMultiplier += _gameData.Options.GarbageIncrease / 60;

			if (_gameData.Options.GarbageCapIncrease > 0)
				_gameData.Options.GarbageCap += _gameData.Options.GarbageCapIncrease / 60;



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
							_gameData.Falling.Init(null, false, NextGenerateMode);
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
				data.amt = FunctionA(data, id1);
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
				amt_value = FunctionA(data, id1);
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

		private int FunctionA(GarbageData data, string? id = null)
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

		private void ProcessLShift(bool value, double subFrameDiff = 1)
		{
			if (!_gameData.LShift || _gameData.RShift && _gameData.LastShift != -1)
				return;

			var subFrameDiff2 = subFrameDiff;
			var dasSomething = Math.Max(0, _gameData.Handling.DAS - _gameData.LDas);

			_gameData.LDas += value ? 0 : subFrameDiff;

			if (_gameData.LDas < _gameData.Handling.DAS && !value)
				return;

			subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasSomething);

			if (_gameData.Falling.Sleep || _gameData.Falling.DeepSleep)
				return;

			var aboutARRValue = 1;
			if (!value)
			{
				_gameData.LDasIter += _gameData.Options.Version >= 15 ? subFrameDiff2 : subFrameDiff;

				if (_gameData.LDasIter < _gameData.Handling.ARR)
					return;

				aboutARRValue = _gameData.Handling.ARR == 0 ? 10 : (int)(_gameData.LDasIter / _gameData.Handling.ARR);

				_gameData.LDasIter -= _gameData.Handling.ARR * aboutARRValue;
			}

			for (var index = 0; index < aboutARRValue; index++)
			{
				if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X - 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Board))
				{
					_gameData.Falling.X--;
					_gameData.Falling.Last = "move";
					_gameData.Falling.Clamped = false;

					if (++_gameData.Falling.LockResets < 15 || _gameData.Options.InfiniteMovement)
						_gameData.Falling.Locking = 0;

				}
				else
				{
					_gameData.Falling.Clamped = true;
				}
			}
		}

		private void ProcessRShift(bool value, double subFrameDiff = 1)
		{
			if (!_gameData.RShift || _gameData.LShift && _gameData.LastShift != 1)
				return;

			var subFrameDiff2 = subFrameDiff;
			var dasSomething = Math.Max(0, _gameData.Handling.DAS - _gameData.RDas);

			_gameData.RDas += value ? 0 : subFrameDiff;

			if (_gameData.RDas < _gameData.Handling.DAS && !value)
				return;

			subFrameDiff2 = Math.Max(0, subFrameDiff2 - dasSomething);
			if (_gameData.Falling.Sleep || _gameData.Falling.DeepSleep)
				return;

			var aboutARRValue = 1;
			if (!value)
			{
				_gameData.RDasIter += _gameData.Options.Version >= 15 ? subFrameDiff2 : subFrameDiff;

				if (_gameData.RDasIter < _gameData.Handling.ARR)
					return;

				aboutARRValue = _gameData.Handling.ARR == 0 ? 10 : (int)(_gameData.RDasIter / _gameData.Handling.ARR);

				_gameData.RDasIter -= _gameData.Handling.ARR * aboutARRValue;
			}

			for (var index = 0; index < aboutARRValue; index++)
			{
				if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X + 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Board))
				{
					_gameData.Falling.X++;
					_gameData.Falling.Last = "move";
					_gameData.Falling.Clamped = false;

					if (++_gameData.Falling.LockResets < 15 || _gameData.Options.InfiniteMovement)
						_gameData.Falling.Locking = 0;

				}
				else
				{
					_gameData.Falling.Clamped = true;
				}
			}
		}

		private void FallEvent(int? value, double subFrameDiff)
		{
			if (_gameData.Falling.SafeLock > 0)
				_gameData.Falling.SafeLock--;

			if (_gameData.Falling.Sleep || _gameData.Falling.DeepSleep)
				return;

			var subframeGravity = _gameData.Gravity * subFrameDiff;

			if (_gameData.SoftDrop)
			{
				if (_gameData.Handling.SDF == (_gameData.Options.Version >= 15 ? 41 : 21))
					subframeGravity = (_gameData.Options.Version >= 15 ? 400 : 20) * subFrameDiff;
				else
				{
					subframeGravity *= _gameData.Handling.SDF;
					subframeGravity = Math.Max(subframeGravity, _gameData.Options.Version >= 13 ?
						0.05 * _gameData.Handling.SDF : 0.42);
				}
			}

			if (value != null)
				subframeGravity = (int)value;

			if (!_gameData.Options.InfiniteMovement &&
				_gameData.Falling.LockResets >= (int)_gameData.Options.LockResets &&
				!IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X,
				_gameData.Falling.Y + 1, _gameData.Falling.R, _gameData.Board))
			{
				subframeGravity = 20;
				_gameData.Falling.ForceLock = true;
			}


			if (!_gameData.Options.InfiniteMovement &&
				_gameData.FallingRotations > (int)_gameData.Options.LockResets + 15)
			{
				subframeGravity += 0.5 * subFrameDiff *
					(_gameData.FallingRotations - ((int)_gameData.Options.LockResets + 15));
			}

			var r = subframeGravity;

			for (; subframeGravity > 0;)
			{
				var ceiledValue = Math.Ceiling(_gameData.Falling.Y);
				if (!HardDrop(Math.Min(1, subframeGravity), r))
				{
					if (value != null)
						_gameData.Falling.ForceLock = true;
					Locking(value != 0 && value != null, subFrameDiff);
					break;
				}

				subframeGravity -= Math.Min(1, subframeGravity);
				if (ceiledValue != Math.Ceiling(_gameData.Falling.Y))
				{
					_gameData.Falling.Last = "fall";
					if (_gameData.SoftDrop)
					{
						//ScoreAdd

					}
				}
			}
		}

		private bool HardDrop(double value, double value2)
		{
			var fallingKouho = Math.Floor(Math.Pow(10, 13) * _gameData.Falling.Y) /
				Math.Pow(10, 13) + value;

			if (fallingKouho % 1 == 0)
				fallingKouho += 0.00001;

			var o = Math.Floor(Math.Pow(10, 13) * _gameData.Falling.Y) / Math.Pow(10, 13) + 1;
			if (o % 1 == 0)
				o -= 0.00002;

			if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, fallingKouho, _gameData.Falling.R, _gameData.Board) &&
				IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, o, _gameData.Falling.R, _gameData.Board))
			{
				var s = _gameData.Falling.HighestY;
				o = _gameData.Falling.Y;

				_gameData.Falling.Y = fallingKouho;
				_gameData.Falling.HighestY = Math.Ceiling(Math.Max(_gameData.Falling.HighestY, fallingKouho));
				_gameData.Falling.Floored = false;
				if (Math.Ceiling(fallingKouho) != Math.Ceiling(o))
				{

				}

				if (fallingKouho > s || _gameData.Options.InfiniteMovement)
					_gameData.Falling.LockResets = 0;
				_gameData.FallingRotations = 0;

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
			_gameData.Falling.Sleep = true;
			//ミノを盤面に適用
			if (!emptyDrop)
			{
				foreach (var pos in ConstData.TETRIMINOS_SHAPES[_gameData.Falling.Type][_gameData.Falling.R])
				{
					_gameData.Board[(int)((pos.x + _gameData.Falling.X - ConstData.TETRIMINO_DIFFS[_gameData.Falling.Type].x) +
						(int)(pos.y + _gameData.Falling.Y - ConstData.TETRIMINO_DIFFS[_gameData.Falling.Type].y) * 10)] = _gameData.Falling.Type;
				}

				if (!sValue && _gameData.Handling.SafeLock > 0)
					_gameData.Falling.SafeLock = 7;

			}

			var istspin = IsTspin();
			var clearedLineCount = ClearLines();

			var announceLine = GetAttackPower(clearedLineCount, istspin);
			IsBoardEmpty();

			if (!announceLine)
			{
				TakeAllDamage();
			}

			_gameData.Falling.Init(null, false, NextGenerateMode);

			OnPiecePlaced?.Invoke(this, EventArgs.Empty);

		}

		internal void CallOnPieceCreated()
		{
			OnPieceCreated?.Invoke(this, EventArgs.Empty);
		}



		public void Move(Action action)
		{
			switch (action)
			{
				case Action.MoveRight:
					if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X + 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Board))
						_gameData.Falling.X++;
					break;

				case Action.MoveLeft:
					if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X - 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Board))
						_gameData.Falling.X--;
					break;

				case Action.RotateLeft:
					var e = _gameData.Falling.R - 1;
					if (e < 0)
						e = 3;
					RotatePiece(e);
					break;

				case Action.RotateRight:
					e = _gameData.Falling.R + 1;
					if (e > 3)
						e = 0;
					RotatePiece(e);
					break;

				case Action.Rotate180:
					e = _gameData.Falling.R + 2;
					if (e > 3)
						e -= 4;
					RotatePiece(e);
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
						if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y + 1, _gameData.Falling.R, _gameData.Board))
							_gameData.Falling.Y++;
						else
							break;
					}
					break;

				case Action.MoveUp:
					if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y - 1, _gameData.Falling.R, _gameData.Board))
						_gameData.Falling.Y--;
					break;

				case Action.MoveDown:
					if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y + 1, _gameData.Falling.R, _gameData.Board))
						_gameData.Falling.Y++;
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
				return _gameData.Falling.SafeLock;

			return null;
		}

		private int ClearLines()
		{
			List<int> list = new List<int>();
			bool flag = true;

			for (int y = FIELD_HEIGHT - 1; y >= 0; y--)
			{
				flag = true;
				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (_gameData.Board[x + y * 10] == (int)MinoKind.Empty)
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
				DownLine(value, _gameData.Board);
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

			return GetFallestPosDiff(_gameData.Falling.Type, _gameData.Falling.X, (int)_gameData.Falling.Y, _gameData.Falling.R);

		}

		public int GetFallestPosDiff(int type, int x, int y, int r)
		{
			if (type == -1)
				return 0;

			int testy = 1;
			while (true)
			{
				if (IsLegalAtPos(type, x, y + testy,
				   r, _gameData.Board))
					testy++;
				else
				{
					testy--;
					break;
				}
			}

			return testy;
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
			if (_gameData.Options.Version >= 15)
				_gameData.Falling.Locking += subframe;
			else
				_gameData.Falling.Locking += 1;

			if (!_gameData.Falling.Floored)
				_gameData.Falling.Floored = true;



			if (_gameData.Falling.Locking > _gameData.Options.LockTime ||
				_gameData.Falling.ForceLock ||
				_gameData.Falling.LockResets > _gameData.Options.LockResets &&
				!_gameData.Options.InfiniteMovement)
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

			if (_gameData.NextBag.Count == 0)
			{
				var array = (int[])ConstData.MINOTYPES.Clone();
				Rng.ShuffleArray(array);
				_gameData.NextBag.AddRange(array);
			}

			if (noszo)
			{
				while (true)
				{
					if (_gameData.NextBag[0] == (int)MinoKind.S ||
						 _gameData.NextBag[0] == (int)MinoKind.Z ||
						_gameData.NextBag[0] == (int)MinoKind.O)
					{
						var temp = _gameData.NextBag[0];
						for (int i = 0; i < _gameData.NextBag.Count - 1; i++)
						{
							_gameData.NextBag[i] = _gameData.NextBag[i + 1];
						}
						_gameData.NextBag[^1] = temp;
					}
					else
						break;
				}

			}


			next[^1] = _gameData.NextBag[0];
			_gameData.NextBag.RemoveAt(0);
			return value;
		}

		private void SwapHold()
		{
			var tempCurrentType = _gameData.Falling.Type;
			_gameData.Falling.Init(_gameData.Hold, true, NextGenerateMode);
			_gameData.Hold = tempCurrentType;
		}

		private string? IsTspin()
		{
			if (_gameData.SpinBonuses == "none")
				return null;

			if (_gameData.SpinBonuses == "stupid")
				throw new NotImplementedException();

			if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y + 1, _gameData.Falling.R, _gameData.Board))
				return null;

			if (_gameData.Falling.Type != (int)MinoKind.T && _gameData.SpinBonuses != "handheld")
			{
				if (_gameData.SpinBonuses == "all")
				{
					if (!(IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X - 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Board) ||
					   IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X + 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Board) ||
					   IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y - 1, _gameData.Falling.R, _gameData.Board) ||
					   IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y + 1, _gameData.Falling.R, _gameData.Board)))
						return "normal";
					else
						return null;
				}
				else
					return null;

			}

			if (_gameData.Falling.Last != "rotate")
				return null;

			var cornerCount = 0;
			var a = 0;

			for (int n = 0; n < 4; n++)
			{
				Vector2[][]? minoCorner = null;

				minoCorner = ConstData.CORNER_TABLE[GetIndex(_gameData.Falling.Type)];

				int GetIndex(int type)
				{
					switch (type)
					{
						case (int)MinoKind.Z:
						case (int)MinoKind.L:
							return type;
						case (int)MinoKind.S:
							return type - 1;
						case (int)MinoKind.J:
						case (int)MinoKind.T:
							return type - 2;

						default: throw new Exception("Unknown type: " + type);

					}
				}

				if (!IsEmptyPos((int)(_gameData.Falling.X + minoCorner[_gameData.Falling.R][n].x),
					(int)(_gameData.Falling.Y + minoCorner[_gameData.Falling.R][n].y), _gameData.Board))
				{
					cornerCount++;

					//AdditinalTableは無理やり追加したものなのでx,yは関係ない
					if (!(_gameData.Falling.Type != (int)MinoKind.T ||
						(_gameData.Falling.R != ConstData.CORNER_ADDITIONAL_TABLE[_gameData.Falling.R][n].x &&
						_gameData.Falling.R != ConstData.CORNER_ADDITIONAL_TABLE[_gameData.Falling.R][n].y)))
						a++;
				}

			}


			if (cornerCount < 3)
				return null;

			var spinType = "normal";

			if (_gameData.Falling.Type == (int)MinoKind.T && a != 2)
				spinType = "mini";

			if (_gameData.Falling.LastKick == 4)
				spinType = "normal";


			return spinType;

		}

		private void ReceiveGarbage(int garbageX, int power)
		{
			for (int y = 0; y < FIELD_HEIGHT; y++)
			{
				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (y + power >= FIELD_HEIGHT)
						break;

					_gameData.Board[x + (y) * 10] = _gameData.Board[x + (y + power) * 10];
				}
			}


			for (int y = FIELD_HEIGHT - 1; y > FIELD_HEIGHT - 1 - power; y--)
			{
				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (x == garbageX)
						_gameData.Board[x + y * 10] = (int)MinoKind.Empty;
					else
						_gameData.Board[x + y * 10] = (int)MinoKind.Garbage;
				}
			}
		}

		private bool GetAttackPower(int clearLineCount, string? isTspin)
		{
			var isBTB = false;

			if (clearLineCount > 0)
			{
				Stats.Combo++;
				Stats.TopCombo = Math.Max(Stats.Combo, Stats.TopCombo);

				if (clearLineCount == 4)
					isBTB = true;
				else
				{
					if (isTspin != null)
						isBTB = true;
				}

				if (isBTB)
				{
					Stats.BTB++;
					Stats.TopBTB = Math.Max(Stats.BTB, Stats.TopBTB);
				}
				else
				{
					Stats.BTB = 0;

				}

			}
			else
			{
				Stats.Combo = 0;
				Stats.CurrentComboPower = 0;
			}


			var garbageValue = 0.0;
			switch (clearLineCount)
			{
				case 0:
					if (isTspin == "mini")
						garbageValue = ConstValue.Garbage.TSPIN_MINI;
					else if (isTspin == "normal")
						garbageValue = ConstValue.Garbage.TSPIN;
					break;

				case 1:
					if (isTspin == "mini")
						garbageValue = ConstValue.Garbage.TSPIN_MINI_SINGLE;
					else if (isTspin == "normal")
						garbageValue = ConstValue.Garbage.TSPIN_SINGLE;
					else
						garbageValue = ConstValue.Garbage.SINGLE;
					break;

				case 2:
					if (isTspin == "mini")
						garbageValue = ConstValue.Garbage.TSPIN_MINI_DOUBLE;
					else if (isTspin == "normal")
						garbageValue = ConstValue.Garbage.TSPIN_DOUBLE;
					else
						garbageValue = ConstValue.Garbage.DOUBLE;
					break;

				case 3:
					if (isTspin != null)
						garbageValue = ConstValue.Garbage.TSPIN_TRIPLE;
					else
						garbageValue = ConstValue.Garbage.TRIPLE;
					break;

				case 4:
					if (isTspin != null)
						garbageValue = ConstValue.Garbage.TSPIN_QUAD;
					else
						garbageValue = ConstValue.Garbage.QUAD;

					break;
			}


			if (clearLineCount > 0 && Stats.BTB > 1)
			{
				if (_gameData.Options.BTBChaining)
				{
					double tempValue;
					if (Stats.BTB - 1 == 1)
						tempValue = 0;
					else
						tempValue = 1 + (Math.Log((Stats.BTB - 1) * ConstValue.Garbage.BACKTOBACK_BONUS_LOG + 1) % 1);


					var btb_bonus = ConstValue.Garbage.BACKTOBACK_BONUS *
						(Math.Floor(1 + Math.Log((Stats.BTB - 1) * ConstValue.Garbage.BACKTOBACK_BONUS_LOG + 1)) + (tempValue / 3));

					//  Debug.WriteLine(Username + " " + (Math.Floor(1 + Math.Log((Stats.BTB - 1) * DataGarbage.BACKTOBACK_BONUS_LOG + 1)) + (tempValue / 3)));
					garbageValue += btb_bonus;

					if ((int)btb_bonus >= 2)
					{
						//AddFire
					}

					if ((int)btb_bonus > _gameData.CurrentBTBChainPower)
					{
						_gameData.CurrentBTBChainPower = (int)btb_bonus;
					}



				}
				else
					garbageValue += ConstValue.Garbage.BACKTOBACK_BONUS;
			}
			else
			{
				if (clearLineCount > 0 && Stats.BTB <= 1)
					_gameData.CurrentBTBChainPower = 0;
			}

			if (Stats.Combo > 1)
			{
				garbageValue *= 1 + ConstValue.Garbage.COMBO_BONUS * (Stats.Combo - 1);
			}

			if (Stats.Combo > 2)
			{
				garbageValue = Math.Max(Math.Log(ConstValue.Garbage.COMBO_MINIFIER *
					(Stats.Combo - 1) * ConstValue.Garbage.COMBO_MINIFIER_LOG + 1), garbageValue);
			}


			int totalPower = (int)(garbageValue * _gameData.Options.GarbageMultiplier);
			if (Stats.Combo > 2)
				Stats.CurrentComboPower = Math.Max(Stats.CurrentComboPower, totalPower);

			if (clearLineCount > 0 && Stats.Combo > 1 && Stats.CurrentComboPower >= 7)
			{

			}

			switch (GameData.Options.GarbageBlocking)
			{
				case "combo blocking":
					if (clearLineCount > 0)
						FightLines(totalPower);
					return clearLineCount > 0;

				case "limited blocking":
					if (clearLineCount > 0)
						FightLines(totalPower);
					return false;

				case "none":
					OffenceAA(totalPower);
					return false;

				default: throw new Exception();
			}
		}

		private void FightLines(int attackLine)
		{

			if (!GameData.Options.HasGarbage)
				return;

			bool count;
			if (GameData.ImpendingDamages.Count == 0)
				count = false;
			else
				count = true;

			var oValue = 0;
			for (; attackLine > 0 && GameData.ImpendingDamages.Count != 0;)
			{
				GameData.ImpendingDamages[0].lines--;
				if (GameData.ImpendingDamages[0].lines == 0)
				{
					GameData.ImpendingDamages.RemoveAt(0);
				}

				attackLine--;
				oValue++;
			}

			if (attackLine > 0)
			{
				if (GameData.Options.Passthrough == "consistent" && GameData.NotYetReceivedAttack > 0)
				{
					//PlaySound
				}
				OffenceAA(attackLine);


			}

		}

		public void OffenceAA(int attackLine)
		{

			attackLine = attackLine / Math.Max(1, 1);
			if (!(attackLine < 1))
			{
				foreach (var target in GameData.Targets)
				{
					var interactionID = ++GameData.InteractionID;
					if (GameData.Options.Passthrough is "zero" or "consistent")
					{
						if (!GameData.GarbageActnowledgements.Outgoing.ContainsKey(target))
							GameData.GarbageActnowledgements.Outgoing.Add(target, new List<GarbageData>());


						GameData.GarbageActnowledgements.Outgoing[target].Add(new GarbageData()
						{
							iid = interactionID,
							amt = attackLine
						});

					}
				}
			}
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

				if (!PushGarbageLine(GameData.ImpendingDamages[0].column, false,
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

		private bool PushGarbageLine(int line, bool falseValue = false, int whatIsThis = 68)
		{
			var newBoardList = new List<int>();
			newBoardList.AddRange((int[])GameData.Board.Clone());

			for (int x = 0; x < FIELD_WIDTH; x++)
			{
				//x+y*10
				if (newBoardList[x] != (int)MinoKind.Empty)
					return false;
			}

			//一番てっぺんを消す
			for (int x = 0; x < FIELD_WIDTH; x++)
				newBoardList.RemoveAt(x);

			var RValueList = new List<int>();

			for (var tIndex = 0; tIndex < FIELD_WIDTH; tIndex++)
			{
				if (tIndex == line)
					RValueList.Add((int)MinoKind.Empty);
				else
					RValueList.Add((int)MinoKind.Garbage);
			}

			newBoardList.AddRange(RValueList);
			GameData.Board = newBoardList.ToArray();
			return true;
		}


		private void IsBoardEmpty()
		{
			int emptyLineCount = 0;
			for (int y = FIELD_HEIGHT - 1; y >= 0; y--)
			{
				if (emptyLineCount >= 2)
					break;

				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (_gameData.Board[x + y * 10] != (int)MinoKind.Empty)
						return;

				}

				emptyLineCount++;
			}

			//PC
			FightLines((int)(ConstValue.Garbage.ALL_CLEAR * _gameData.Options.GarbageMultiplier));
		}

		/// <summary>
		/// Cancel received attack power.
		/// </summary>
		/// <param name="lines">Power</param>
		//private void CancelAttack(int lines)
		//{
		//    while (Garbages.Count != 0 && lines != 0)
		//    {
		//        if (Garbages[0].Power <= lines)
		//        {
		//            lines -= Garbages[0].Power;
		//            if (Garbages[0].ConfirmedFrame == -1)
		//                _historyInteraction.Add(Garbages[0]);
		//            Garbages.RemoveAt(0);
		//        }
		//        else
		//        {
		//            Garbages[0].Power -= lines;
		//            lines = 0;
		//        }
		//    }

		//}
	}
}
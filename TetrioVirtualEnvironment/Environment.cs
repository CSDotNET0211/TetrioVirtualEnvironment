using System.Diagnostics;
using System.Text.Json;
using TetrReplayLoader;
using TetrReplayLoader.JsonClass.Event;

namespace TetrioVirtualEnvironment
{
	public class Environment
	{
		//DOTO: これとかapmに関する情報は関数でも作る pressedkeylist中身あったっけ
		/// <summary>
		/// Pressed key data
		/// </summary>
		public List<string> PressedKeyList { get; private set; } = new();
		public const int FIELD_WIDTH = 10;
		public const int FIELD_HEIGHT = 40;

		public static int FIELD_SIZE => FIELD_WIDTH * FIELD_HEIGHT;

		// -- Events --
		public event EventHandler? OnPiecePlaced;
		public event EventHandler? OnPieceCreated;


		private List<Garbage> _historyInteraction;
		public int RefreshNextCount { get; private set; }
		public bool InfinityHold { get; set; } = false;
		public string? Username { get; }

		/// <summary>
		/// This is used to judge spin bonus.
		/// Basically, used as Tspin.
		/// AllSpin uses all data.
		/// </summary>
		public static readonly Vector2[][][] CORNER_TABLE =
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

		public static readonly Vector2[][] CORNER_ADDITIONAL_TABLE =
			   {
				   new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
				   new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
				   new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
				   new[] { new Vector2(3, 0), new Vector2(0, 1), new Vector2(1, 2), new Vector2(2, 3) },
			   };



		/// <summary>
		/// Tetrimino Array
		/// </summary>
		public static readonly Vector2[][][] TETRIMINOS =
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
		public static readonly Vector2[] TETRIMINO_DIFF = { new(1, 1), new(1, 1), new(0, 1), new(1, 1), new(1, 1), new(1, 1), new(1, 1) };

		public static readonly int[] MINOTYPES = { (int)MinoKind.Z, (int)MinoKind.L, (int)MinoKind.O, (int)MinoKind.S, (int)MinoKind.I, (int)MinoKind.J, (int)MinoKind.T, };

		/// <summary>
		/// GarbageList
		/// </summary>
		public List<Garbage> Garbages { get; private set; }
		/// <summary>
		/// GarbageList for Garbage.StateEnum.Attack
		/// attak event maybe caused by lag?
		/// </summary>
		public List<Garbage> GarbagesInterrupt { get; private set; }
		/// <summary>
		/// Kickset of SRS+
		/// </summary>
		public Dictionary<string, Vector2[]> KICKSET_SRSPLUS { get; }
		/// <summary>
		/// Kickset of SRS+ I-piece
		/// </summary>
		public Dictionary<string, Vector2[]> KICKSET_SRSPLUSI { get; }

		internal GameData _gameData;
		public GameData GameData => _gameData;
		public Stats Stats { get; private set; }
		public Rng Rng { get; } = new();
		public EnvironmentModeEnum EnvironmentMode { get; }
		/// <summary>
		/// event data of 'full'.use it for reset the game.
		/// </summary>
		public EventFull EventFull { get; }
		public DataForInitialize DataForInitialize { get; }
		public int NextSkipCount;
		/// <summary>
		/// for process events at same frame.
		/// </summary>
		public int CurrentIndex { get; private set; }

		public int CurrentFrame { get; private set; }

		public enum EnvironmentModeEnum
		{
			Limited,
			Seed
		}
		public enum MinoKind
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

		public enum Rotation
		{
			Zero,
			Right,
			Turn,
			Left
		}
		/// <summary>
		/// for AI
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

		/// <summary>
		/// 右か左回転か
		/// </summary>
		public enum Rotate : byte
		{
			Right,
			Left
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
		public Environment(EventFull envData, EnvironmentModeEnum envMode, string? username, DataForInitialize? dataForInitialize = null, int nextSkipCount = 0)
		{
			dataForInitialize ??= new DataForInitialize();

			Username = username;

			EnvironmentMode = envMode;
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
		public void ResetGame(EventFull envData, EnvironmentModeEnum envMode, DataForInitialize dataForInitialize, int nextSkipCount = 0)
		{
			RefreshNextCount = 0;
			_historyInteraction = new List<Garbage>();
			Garbages = new List<Garbage>();
			GarbagesInterrupt = new List<Garbage>();
			CurrentFrame = 0;
			CurrentIndex = 0;
			Stats = new Stats();

			new GameData(envMode, envData, this, out _gameData, nextSkipCount, dataForInitialize);
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

				if (value.State == Garbage.StateEnum.Ready)
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
			if (EnvironmentMode == EnvironmentModeEnum.Limited)
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


		public static bool IsEmptyPos(int x, int y, int[] field)
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
			var positions = TETRIMINOS[type][rotation];
			var diff = TETRIMINO_DIFF[type];

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
		public static bool IsLegalAtPos(int type, int x, double y, int rotation, int[] field)
		{
			var positions = TETRIMINOS[type][rotation];
			var diff = TETRIMINO_DIFF[type];

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
				_gameData.Field))
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
					i2, rotation, _gameData.Field))
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
		public void ProcessShift(bool value, double subFrameDiff)
		{
			ProcessLShift(value, subFrameDiff);
			ProcessRShift(value, subFrameDiff);
		}

		public bool Update(List<Event>? events = null)
		{
			_gameData.SubFrame = 0;

			if (events != null && !UpdateEvent(events))
				return false;

			CurrentFrame++;
			ProcessShift(false, 1 - _gameData.SubFrame);
			FallEvent(null, 1 - _gameData.SubFrame);

			if (events == null)
				return true;

			CheckGarbage();

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

		private void CheckGarbage()
		{
			foreach (var garbage in Garbages)
			{
				if (garbage.ConfirmedFrame + 20 != CurrentFrame) continue;
				if (garbage.State == Garbage.StateEnum.InteractionConfirm)
					garbage.State = Garbage.StateEnum.Ready;
			}

			for (int i = GarbagesInterrupt.Count - 1; i >= 0; i--)
			{
				if (GarbagesInterrupt[i].ConfirmedFrame + 20 != CurrentFrame) continue;
				GarbagesInterrupt[i].State = Garbage.StateEnum.Ready;
				Garbages.Add(GarbagesInterrupt[i]);
				GarbagesInterrupt.RemoveAt(i);
			}
		}

		/// <summary>
		/// Update game during events[CurrentIndex].frame == CurrentFrame 
		/// </summary>
		/// <param name="events"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public bool UpdateEvent(List<Event> events)
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
							_gameData.Falling.Init(null,false, EnvironmentMode);
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

						//This environment support solo or 2 player only so it is not used.
						case "targets":
							break;

						case "ige":
							var data = JsonSerializer.Deserialize<EventIge>(events[CurrentIndex].data.ToString());

							//when interaction_confirm is called, interaction is called before in most case.
							//
							if (data?.data.type == "interaction_confirm")
							{
								var notConfirmed = true;
								foreach (var garbage in Garbages)
								{
									if (garbage.SentFrame == data.data.sent_frame && garbage.State == Garbage.StateEnum.Interaction)
									{
										garbage.State = Garbage.StateEnum.InteractionConfirm;
										garbage.ConfirmedFrame = (int)events[CurrentIndex].frame;
										notConfirmed = false;
										break;
									}

								}

								if (notConfirmed)
								{
									var flag2 = true;

									var historyInteractionCount = _historyInteraction.Count;
									for (int i = 0; i < historyInteractionCount; i++)
									{
										if (_historyInteraction[0].SentFrame == data.data.sent_frame)
										{
											flag2 = false;
											_historyInteraction.RemoveAt(0);
											break;
										}
									}

									if (flag2)
									{
										GarbagesInterrupt.Add(new Garbage(data.frame,
										(int)events[CurrentIndex].frame, data.data.sent_frame, data.data.data.column,
										data.data.data.amt, Garbage.StateEnum.Attack));
									}



								}
							}
							else if (data.data.type == "interaction")
							{
								Garbages.Add(new Garbage(data.frame, -1, data.data.sent_frame, data.data.data.column, data.data.data.amt, Garbage.StateEnum.Interaction));
							}
							else if (data.data.type == "attack")
							{
								//attack event will caused by lag?
								//
								GarbagesInterrupt.Add(new Garbage(data.frame, (int)events[CurrentIndex].frame, (int)data.data.sent_frame, data.data.column, (int)data.data.lines, Garbage.StateEnum.Attack));
							}
							else if (data.data.type == "kev")
							{
								//I forget about this event but there is no problem because these is empty.
							}
							else throw new Exception("unknown InGameEvent");

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
				if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X - 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Field))
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
				if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X + 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Field))
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

		public void FallEvent(int? value, double subFrameDiff)
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
				_gameData.Falling.Y + 1, _gameData.Falling.R, _gameData.Field))
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
					CheckForcePlacePiece(value != 0 && value != null, subFrameDiff);
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

			if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, fallingKouho, _gameData.Falling.R, _gameData.Field) &&
				IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, o, _gameData.Falling.R, _gameData.Field))
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

		public void PiecePlace(bool sValue, bool forceEmptyDrop = false)
		{
			_gameData.Falling.Sleep = true;
			//ミノを盤面に適用
			if (!forceEmptyDrop)
			{
				foreach (var pos in TETRIMINOS[_gameData.Falling.Type][_gameData.Falling.R])
				{
					_gameData.Field[(int)((pos.x + _gameData.Falling.X - TETRIMINO_DIFF[_gameData.Falling.Type].x) +
						(int)(pos.y + _gameData.Falling.Y - TETRIMINO_DIFF[_gameData.Falling.Type].y) * 10)] = _gameData.Falling.Type;
				}

				if (!sValue && _gameData.Handling.SafeLock > 0)
					_gameData.Falling.SafeLock = 7;

			}

			var istspin = IsTspin();
			var clearedLineCount = ClearLines();

			GetAttackPower(clearedLineCount, istspin);
			IsBoardEmpty();

			_gameData.Falling.Init(null,false, EnvironmentMode);

			OnPiecePlaced?.Invoke(this, EventArgs.Empty);

		}

		public void CallOnPieceCreated()
		{
			OnPieceCreated?.Invoke(this, EventArgs.Empty);
		}


		public void Move(Action action)
		{
			switch (action)
			{
				case Action.MoveRight:
					if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X + 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Field))
						_gameData.Falling.X++;
					break;

				case Action.MoveLeft:
					if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X - 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Field))
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
						if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y + 1, _gameData.Falling.R, _gameData.Field))
							_gameData.Falling.Y++;
						else
							break;
					}
					break;

				case Action.MoveUp:
					if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y - 1, _gameData.Falling.R, _gameData.Field))
						_gameData.Falling.Y--;
					break;

				case Action.MoveDown:
					if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y + 1, _gameData.Falling.R, _gameData.Field))
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
					if (_gameData.Field[x + y * 10] == (int)MinoKind.Empty)
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
				DownLine(value, _gameData.Field);
			}

			return list.Count;

		}

		public static Vector2[] GetMinoPos(int type, int x, int y, int r)
		{
			var positions = (Vector2[])TETRIMINOS[type][r].Clone();
			var diff = TETRIMINO_DIFF[type];

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
				   r, _gameData.Field))
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

		private void CheckForcePlacePiece(bool value = false, double subframe = 1)
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
				PiecePlace(value);
			}
		}

		public int RefreshNext(List<int> next, bool noszo)
		{
			RefreshNextCount++;
			var value = next[0];

			for (int i = 0; i < next.Count - 1; i++)
			{
				next[i] = next[i + 1];
			}

			if (_gameData.NextBag.Count == 0)
			{
				var array = (int[])MINOTYPES.Clone();
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
			_gameData.Falling.Init(_gameData.Hold,true,EnvironmentMode);
			_gameData.Hold = tempCurrentType;
		}

		private string? IsTspin()
		{
			if (_gameData.SpinBonuses == "none")
				return null;

			if (_gameData.SpinBonuses == "stupid")
				throw new NotImplementedException();

			if (IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y + 1, _gameData.Falling.R, _gameData.Field))
				return null;

			if (_gameData.Falling.Type != (int)MinoKind.T && _gameData.SpinBonuses != "handheld")
			{
				if (_gameData.SpinBonuses == "all")
				{
					if (!(IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X - 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Field) ||
					   IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X + 1, _gameData.Falling.Y, _gameData.Falling.R, _gameData.Field) ||
					   IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y - 1, _gameData.Falling.R, _gameData.Field) ||
					   IsLegalAtPos(_gameData.Falling.Type, _gameData.Falling.X, _gameData.Falling.Y + 1, _gameData.Falling.R, _gameData.Field)))
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

				minoCorner = CORNER_TABLE[GetIndex(_gameData.Falling.Type)];

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
					(int)(_gameData.Falling.Y + minoCorner[_gameData.Falling.R][n].y), _gameData.Field))
				{
					cornerCount++;

					//AdditinalTableは無理やり追加したものなのでx,yは関係ない
					if (!(_gameData.Falling.Type != (int)MinoKind.T ||
						(_gameData.Falling.R != CORNER_ADDITIONAL_TABLE[_gameData.Falling.R][n].x &&
						_gameData.Falling.R != CORNER_ADDITIONAL_TABLE[_gameData.Falling.R][n].y)))
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

		public void ReceiveGarbage(int garbageX, int power)
		{
			for (int y = 0; y < FIELD_HEIGHT; y++)
			{
				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (y + power >= FIELD_HEIGHT)
						break;

					_gameData.Field[x + (y) * 10] = _gameData.Field[x + (y + power) * 10];
				}
			}


			for (int y = FIELD_HEIGHT - 1; y > FIELD_HEIGHT - 1 - power; y--)
			{
				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (x == garbageX)
						_gameData.Field[x + y * 10] = (int)MinoKind.Empty;
					else
						_gameData.Field[x + y * 10] = (int)MinoKind.Garbage;
				}
			}
		}

		public void GetAttackPower(int clearLineCount, string? isTspin)
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
				//そもそもAddFireって？
				//AddFire
			}

			//火力の相殺をする
			CancelAttack(totalPower);
			//リプレイだと火力送信する必要なし、相殺のみ
			if (clearLineCount > 0)
			{
				//FightLines
			}
			else
			{
				TakeAllDamage();
			}

		}

		private void TakeAllDamage()
		{
			var receivedDamage = 0;

			var receiveGarbageCount = Garbages.CountCanReceive();
			for (int i = 0; i < receiveGarbageCount; i++)
			{
				receivedDamage += Garbages[0].Power;

				if (receivedDamage > _gameData.Options.GarbageCap)
				{
					//receive
					var receivedValue = (int)_gameData.Options.GarbageCap - (receivedDamage - Garbages[0].Power);
					ReceiveGarbage(Garbages[0].PosX, receivedValue);
					Garbages[0].Power -= receivedValue;
					receivedDamage = (int)_gameData.Options.GarbageCap;
					break;
				}
				else
				{
					//receive
					ReceiveGarbage(Garbages[0].PosX, Garbages[0].Power);
					Garbages.RemoveAt(0);
				}
			}

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
					if (_gameData.Field[x + y * 10] != (int)MinoKind.Empty)
						return;

				}

				emptyLineCount++;
			}

			//PC
			CancelAttack((int)(ConstValue.Garbage.ALL_CLEAR * _gameData.Options.GarbageMultiplier));



		}

		/// <summary>
		/// In replay, received power is already exists so just cancel check.
		/// </summary>
		/// <param name="lines">cancel power</param>
		private void CancelAttack(int lines)
		{
			while (Garbages.Count != 0 && lines != 0)
			{
				if (Garbages[0].Power <= lines)
				{
					lines -= Garbages[0].Power;
					if (Garbages[0].ConfirmedFrame == -1)
						_historyInteraction.Add(Garbages[0]);
					Garbages.RemoveAt(0);
				}
				else
				{
					Garbages[0].Power -= lines;
					lines = 0;
				}
			}

		}
	}
}
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUIManagerMobile : MMSingleton<GUIManagerMobile>
{
		[Tooltip("the main canvas")]
		public Canvas MainCanvas;

		[Tooltip("HUD (Health, Inventory, Points...)")]
		public GameObject HUD;

		[Tooltip("PauseScreen UI")]
		public GameObject PauseScreen;

		[Tooltip("DeathScreen UI")]
		public GameObject DeathScreen;

		[Tooltip("DungeonScreen UI")]
		public GameObject DungeonScreen;

		[Tooltip("Mobile Buttons")]
		public CanvasGroup Buttons;

		[Tooltip("Mobile JoyStick")]
		public CanvasGroup Joystick;

        [Tooltip("Gold")]
		public TextMeshProUGUI Gold;

		[Tooltip("Dungeon Floor")]
		public TextMeshProUGUI FloorText;

		protected float _initialJoystickAlpha;
		protected float _initialButtonsAlpha;
		protected bool _initialized = false;

		/// <summary>
		/// Statics initialization to support enter play modes
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		protected static void InitializeStatics()
		{
			_instance = null;
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			Initialization();
		}

		protected virtual void Initialization()
		{
			if (_initialized)
			{
				return;
			}

			if (Joystick != null)
			{
				_initialJoystickAlpha = Joystick.alpha;
			}
			if (Buttons != null)
			{
				_initialButtonsAlpha = Buttons.alpha;
			}

			_initialized = true;
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		protected virtual void Start()
		{
			SetPauseScreen(false);
			SetDeathScreen(false);

			if (DungeonManager.HasInstance)
			{
				SetDungeonScreen(true);
				RefreshFloor();
			}
			else
			{
				SetDungeonScreen(false);
			}
		}

		/// <summary>
		/// ポーズ画面の切り替え
		/// </summary>
		/// <param name="state">If set to <c>true</c>, sets the pause.</param>
		public virtual void SetPauseScreen(bool state)
		{
			if (PauseScreen != null)
			{
				PauseScreen.SetActive(state);
				EventSystem.current.sendNavigationEvents = state;
			}
		}

		/// <summary>
		/// 死亡画面の切り替え
		/// </summary>
		/// <param name="state">If set to <c>true</c>, sets the pause.</param>
		public virtual void SetDeathScreen(bool state)
		{
			if (DeathScreen != null)
			{
				DeathScreen.SetActive(state);
				EventSystem.current.sendNavigationEvents = state;
			}
		}

		/// <summary>
		/// ダンジョン画面の切り替え
		/// </summary>
		/// <param name="state">If set to <c>true</c>, sets the pause.</param>
		public virtual void SetDungeonScreen(bool state)
		{
			if (DungeonScreen != null)
			{
				DungeonScreen.SetActive(state);
				EventSystem.current.sendNavigationEvents = state;
			}
		}

		/// <summary>
		/// フロア表示の更新
		/// </summary>
		public virtual void RefreshFloor()
		{
			if (FloorText != null)
			{
                if (DungeonManager.Instance.CurrentFloor <= DungeonManager.Instance.MaxFloor)
                {
                    FloorText.text = DungeonManager.Instance.CurrentFloor.ToString() + "F";
                }
				else
                {
                    FloorText.text = "???";
                }
			}
		}

		/// <summary>
		/// InputManagerから呼び出される. コントローラーの表示切替
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		/// <param name="movementControl">Movement control.</param>
		public virtual void SetMobileControlsActive(bool state, InputManager.MovementControls movementControl = InputManager.MovementControls.Joystick)
		{
			Initialization();

			if (Joystick != null)
			{
				Joystick.gameObject.SetActive(state);
				if (state && movementControl == InputManager.MovementControls.Joystick)
				{
					Joystick.alpha=_initialJoystickAlpha;
				}
				else
				{
					Joystick.alpha=0;
					Joystick.gameObject.SetActive (false);
				}
			}

			if (Buttons != null)
			{
				Buttons.gameObject.SetActive(state);
				if (state)
				{
					Buttons.alpha=_initialButtonsAlpha;
				}
				else
				{
					Buttons.alpha=0;
					Buttons.gameObject.SetActive (false);
				}
			}
		}
}
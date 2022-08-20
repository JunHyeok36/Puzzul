using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
	static GameManager instance = null;
	public static GameManager Instance {
		get {
			if(instance == null){
				instance = (GameManager)FindObjectOfType(typeof(GameManager));
			}
			return instance;
		}
	}

    public const byte X_MAX = 8, Y_MAX = 12;

	AudioSource audioSource;

	[Space]
	[SerializeField] GameObject canvas = null;
	[SerializeField] Animator user_interface_anim = null;

	[Header("Field")]
	[SerializeField] GameObject field_object = null;
	public GameObject FieldObject { get { return field_object; } }
	[SerializeField] GameObject animatingField_object = null;
	public GameObject AnimatingFieldObject { get { return animatingField_object; } }
	[SerializeField] Image hint_field_img = null;
	[SerializeField] RectTransform hint_field_rectTransform = null;
	AnimationalNumber<float> hint_field_alpha = new AnimationalNumber<float>(0f, 5f, 0, 0f);

	Piece[,] field_piece = new Piece[X_MAX,Y_MAX];

	[Header("UserInterface")]
	[SerializeField] TextMeshProUGUI highscore_txt = null;
	bool is_highscoreTxt_showed = false;
	[SerializeField] TextMeshProUGUI score_txt = null;
	[SerializeField] GameObject score_box = null;
	AnimationalComponent score_anim = new AnimationalComponent(new Vector3(300, 75, 0), new Vector3(300, 75, 0), 10f);

	[SerializeField] TextMeshProUGUI combo_mutiple_txt = null;
	[SerializeField] Image combo_duration_img = null;
	[SerializeField] Image gameover_duration_img = null;

	[Header("GameoverPanel")]
	[SerializeField] GameObject gameover_panel = null;
	[SerializeField] TextMeshProUGUI score_result_txt = null;

	[Header("CautionPanel")]
    [SerializeField] GameObject caution_panel = null;
    [SerializeField] TextMeshProUGUI caution_txt = null;

	[Header("PausePanel")]
	[SerializeField] GameObject pause_panel = null;
	[SerializeField] TextMeshProUGUI pause_content_txt = null;

	[Header("GooglePlayServices")]
	[SerializeField] GameObject recodeLeaderBoard_object = null;

	float lerp = 0;

	//[Header("Score")]//
	ulong score = 0, score_target = 0, score_high = 0;
	int coin = 0, using_pause = 0;
	float combo_mutiple = 1f;
	bool is_combo = false, is_about_end = false, is_gameover = false, is_score_reduced = false, is_paused = false;

	int combo_chain = 1;
	/////////////////////
	
	[Header("UserSetting")]
	[SerializeField] int normal_combo = 100;

	[SerializeField] AnimationalNumber<float> hold_score = new AnimationalNumber<float>(5f, 5f, 0, 1f);// init, weight(int), x, repeat 
	[SerializeField] AnimationalNumber<float> combo_duration = new AnimationalNumber<float>(5f, 3f, 0f, 0f);
	[SerializeField] AnimationalNumber<float> gameover_duration = new AnimationalNumber<float>(8f, 8f, 0f, 0f);
	float gameover_duration_additional = 0f;
	float combo_duration_additional = 0f;
	float hold_decrease_score = 0f;
	float combo_mutiple_weight = 0.2f;

	[SerializeField] Image mainRune_coolTime_img = null;
	[SerializeField] float mainRune_coolTime = 0f, mainRune_coolTime_init = 0f;
	bool is_need_mainRune_coolTime = false;
	///////////////////////////

	bool isAnimating = false, isRaining = false, isShowedHint = false;
	[SerializeField] bool dev_mode = false;

	bool invokePossibleFieldDenied = false, formingPossibleField = false, invokePossibleDenied = false, formingPossible = false;

	Axis axis_hint = Axis.ZERO;
	const int range_hint = 3;

	List<Piece> piece_remove_list = new List<Piece>(); 
	List<PieceCombo> piece_combo_list = new List<PieceCombo>();

	//List<Axis> axis_raining_list = new List<Axis>();

	delegate void Explode(int min_range, int max_range);
	Explode reservedExplode;
	int explode_min_range = 1, explode_max_range = 1;

	void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	void Start() {
		RuneInitSetting();

		Data data = DataManager.Load();

		score_high = data.GetHighScore();

		InitField(true);

		//InvokeRepeating("HoldScore", hold_score.init, hold_score.target);
	}

	void RuneInitSetting() {
		MainRune mainRune = ListManager.Instance.mainRune;
		List<SubRune> subRune_list = ListManager.Instance.subRune_list;

		normal_combo = 100;
		combo_mutiple_weight = 0.2f;

		switch(mainRune) {
			case MainRune.TimeBreaker :
				is_need_mainRune_coolTime = true;
				combo_duration_additional += 2f;
				mainRune_coolTime_init = 10f;
				break;
			case MainRune.Opportunist :
				is_need_mainRune_coolTime = true;
				mainRune_coolTime_init = 15f;
				break;
			default :	
				break;
		}

		for(int i = 0; i < subRune_list.Count; i++) {
			switch(subRune_list[i]) {
				case SubRune.AScore :
					normal_combo += 200;
					break;
				case SubRune.BScore :
					normal_combo += 100;
					break;
				case SubRune.CScore :
					normal_combo += 50;
					break;
				case SubRune.DScore :
					normal_combo += 10;
					break;
				case SubRune.ATime :
					gameover_duration_additional += 3f;
					break;
				case SubRune.BTime :
					gameover_duration_additional += 2f;
					break;
				case SubRune.CTime :
					gameover_duration_additional += 1f;
					break;
				case SubRune.ACombo :
					combo_mutiple_weight += 0.5f;
					break;
				case SubRune.BCombo :
					combo_mutiple_weight += 0.3f;
					break;
				case SubRune.CCombo :
					combo_mutiple_weight += 0.1f;
					break;
				default :
					break;
			}
		}
	}

	public void ContinueGame() { // test method
		is_gameover = false;
		gameover_panel.SetActive(false);

		gameover_duration.currnet = gameover_duration.init;
	}

	void Update() {
		if(!is_paused) {
			if(isRaining || isAnimating) {
				Time.timeScale = 0.7f;
			} else {
				Time.timeScale = 1f;
			}
		}

		UpdateEndTimer();
		UpdateCombo();
		UpdateScore();
		UpdateHintObject();

		void UpdateEndTimer() {
			if(is_about_end) {
				if(!isShowedHint && gameover_duration.currnet < gameover_duration.init * 0.75f) {
					SetHint();
				}
				if(gameover_duration.currnet > 0f) {
					gameover_duration.currnet -= Time.deltaTime;
					gameover_duration_img.fillAmount = gameover_duration.currnet / gameover_duration.init;
				} else {
					if(!dev_mode) {
						Gameover();
					}
				}
			}
			if(is_need_mainRune_coolTime) {
				mainRune_coolTime -= Time.deltaTime;
				mainRune_coolTime_img.fillAmount = mainRune_coolTime / mainRune_coolTime_init;
			}
		}

		void UpdateCombo() {
			if(combo_duration.currnet > 0f) {
				combo_duration.currnet -= Time.deltaTime;
				combo_duration_img.fillAmount = combo_duration.currnet / combo_duration.init;
			} else {
				UnsetCombo();
			}
			RectTransform rectTransform_score_box = score_box.GetComponent<RectTransform>();
			rectTransform_score_box.anchoredPosition = Vector3.Lerp(rectTransform_score_box.anchoredPosition, score_anim.position_target, Time.deltaTime * score_anim.position_speed);
		}

		void UpdateScore() {
			score = (ulong)System.Math.Ceiling(Mathf.Lerp(score, score_target, lerp));
			lerp += Time.deltaTime;
			if(lerp > 0.6f) {
				score = score_target;
			}
			score_txt.text = System.String.Format("{0:#,0}", score);
		}

		void UpdateHintObject() {
			hint_field_alpha.currnet = Mathf.Lerp(hint_field_alpha.currnet, hint_field_alpha.target, Time.deltaTime * hint_field_alpha.weight);
			hint_field_img.color = new Color(1f, 1f, 1f, hint_field_alpha.currnet);
		}
	}

	void SetHint() {
		hint_field_alpha.target = 0.15f;

		if(axis_hint.x < range_hint - 1) {
			axis_hint.x = range_hint - 1;
		} else if(X_MAX - range_hint < axis_hint.x) {
			axis_hint.x = X_MAX - range_hint;
		}

		if(axis_hint.y < range_hint - 1) {
			axis_hint.y = range_hint - 1;
		} else if(Y_MAX - range_hint < axis_hint.y) {
			axis_hint.y = Y_MAX - range_hint;
		}

		hint_field_rectTransform.anchoredPosition = AxisToVector(axis_hint);

		isShowedHint = true;
	}

	void UnsetHint() {
		hint_field_alpha.target = 0f;

		isShowedHint = false;
	}

	void Gameover() {
		if(is_gameover) {
			return;
		}
		gameover_panel.SetActive(true);
		is_gameover = true;
		ListManager.Instance.SetGameOverBGM();

		score_result_txt.text = $"<size=125>{ListManager.GetLocalizedString("Score")}</size>\n{score_target}";
		coin += (int)(score_target / 50_000);

		if(score_high < score_target) {
			score_result_txt.text += $"\n<size=100><color=#FFFF00>{ListManager.GetLocalizedString("NewHighScore")}</color></size>";
		}
		if(coin > 0) {
			score_result_txt.text += $"\n<size=125>+<sprite=0>{coin}</size>";
		}

		if(GPGSManager.Instance.IsAuthenticated) {
			GPGSManager.Instance.ReportLeaderboard(GPGSIds.leaderboard_ranking, (long)score_target, (success) => {
				if(success && score_high > score_target) {
					recodeLeaderBoard_object.SetActive(true);
				}
			});
		}

		CancelInvoke("HoldScore");

		SaveData();
	}

	public void GoToMainMenu() {
		ListManager.Instance.SetOutGameBGM();
		SceneLoader.Load("MainMenu");
	}

	public void GameoverInPause() {
		is_gameover = true;
		CancelInvoke("HoldScore");

		Time.timeScale = 1f;

		GoToMainMenu();
	}

	public void GoToRuneSetting() {
		ListManager.Instance.SetOutGameBGM();
		SceneLoader.Load("RuneSetting");
	}

	void HoldScore() {
		ulong score_condition = score_target + (ulong)hold_score.weight - (ulong)Random.Range(0, hold_score.weight);
		
		if(score_condition < 0) {
			score_target = 0;
		} else {
			score_target += (ulong)hold_score.weight - (ulong)Random.Range(0, hold_score.weight);
		}

		if(score_target > 1_000_000_000) {
			gameover_duration.init = 1f + gameover_duration_additional;
			combo_duration.init = 1f + combo_duration_additional;
			combo_duration.weight = 0.5f;
			hold_score.weight = -1000f + hold_decrease_score;
		} else if(score_target > 100_000_000) {
			gameover_duration.init = 2f + gameover_duration_additional;
			combo_duration.init = 2f + combo_duration_additional;
			combo_duration.weight = 1f;
			hold_score.weight = -500f + hold_decrease_score;
		} else if(score_target > 10_000_000) {
			gameover_duration.init = 4f + gameover_duration_additional;
			combo_duration.init = 4f + combo_duration_additional;
			combo_duration.weight = 2f;
			hold_score.weight = -100f + hold_decrease_score;
		} else if(score_target > 1_000_000) {
			gameover_duration.init = 6f + gameover_duration_additional;
			combo_duration.init = 5f + combo_duration_additional;
			hold_score.weight = -50f + hold_decrease_score;
		} else {
			gameover_duration.init = 8f + gameover_duration_additional;
			combo_duration.init = 6f + combo_duration_additional;
			hold_score.weight = -5f + hold_decrease_score;
		}
	}

	void SetScore(List<Piece> piece_list) {
		lerp = 0;
		ulong score_add = 0;

		if(piece_list.Count > 3) {
			score_add = (ulong)(normal_combo * (piece_list.Count - 3) * 5);
		} else {
			score_add = (ulong)normal_combo;
		}

		if(is_score_reduced) {
			score_add = (ulong)(score_add * 0.25f);
			is_score_reduced = false;
			score_target += (ulong)(score_add * combo_mutiple);
		} else {
			score_target += (ulong)(score_add * combo_mutiple * combo_chain * combo_chain);
		}

		
		//SetScoreFigure(piece_list[0].Axis, score_add);
		user_interface_anim.SetTrigger("Combo");
		audioSource.clip = ListManager.Instance.ComboAudioClip;
		audioSource.Play();

		SetCombo();

		combo_chain++;

		if(ListManager.Instance.mainRune == MainRune.Opportunist && piece_list.Count > 5 && mainRune_coolTime <= 0f) {
			reservedExplode = new Explode(ExplodeAll);
			mainRune_coolTime = mainRune_coolTime_init;
		}

		if(score_high != 0 && !is_highscoreTxt_showed && score_target > score_high) {
			highscore_txt.gameObject.SetActive(true);
			is_highscoreTxt_showed = true;
			Invoke("UnsetHighScoreText", 4f);
		}

		void SetCombo() {
			if(!is_combo) {
				is_combo = true;
				is_about_end = false;

				combo_duration.currnet = combo_duration.init;
				gameover_duration.currnet = gameover_duration.init;
				gameover_duration_img.fillAmount = gameover_duration.currnet / gameover_duration.init;
				score_anim.position_target = new Vector3(-300, 75, 0);
			} else {
				combo_duration.currnet += combo_duration.weight;
			}
			
			if(ListManager.Instance.mainRune == MainRune.Opportunist && piece_list.Count > 3) {
				combo_mutiple += combo_mutiple_weight * ((!is_score_reduced) ? combo_chain * 4f : 4f);
			} else {
				combo_mutiple += combo_mutiple_weight * ((!is_score_reduced) ? combo_chain : 1f);
			}

			combo_mutiple_txt.text = System.String.Format("×{0:0.0}", combo_mutiple);

			if(combo_duration.init * 1.05f < combo_duration.currnet) {
				if(ListManager.Instance.mainRune == MainRune.TimeBreaker && mainRune_coolTime <= 0f) {
					reservedExplode = new Explode(ExplodeCircle);
					explode_min_range = 1; explode_max_range = 3;
					mainRune_coolTime = mainRune_coolTime_init;
				}
			}
			if(combo_duration.init * 2f < combo_duration.currnet) {
				combo_duration.currnet = combo_duration.init * 2f;
			}
		}

		void SetScoreFigure(Axis axis_target, int figure_value) {
			GameObject figure_object = ObjectManager.Instance.GetObject(ObjectList.Figure);
			figure_object.transform.SetParent(canvas.transform, false);

			figure_object.GetComponent<Figure>().SetInfo(axis_target, figure_value);
		}
	}

	void UnsetHighScoreText() {
		highscore_txt.gameObject.SetActive(false);
	}

	void UnsetCombo() {
		combo_duration.currnet = 0f;

		combo_mutiple = 1f;

		is_combo = false;

		score_anim.position_target = new Vector3(300, 75, 0);

		SetEndTimer();
	}

	void SetEndTimer() {
		if(!is_about_end) {
			is_about_end = true;
			gameover_duration.currnet = gameover_duration.init;
		}
	}

	void InitField(bool first_time = false) {
		if(formingPossibleField) {
			invokePossibleFieldDenied = true;
		}
		formingPossibleField = true;
		isAnimating = true;
		for(byte y = 0; y < Y_MAX; y++) {
			for(byte x = 0; x < X_MAX; x++) {
				if(!first_time) {
					Piece piece_target = field_piece[x, y];

					piece_target.RemoveAnimation();
				}

				byte piece_id = ListManager.Instance.GetRandomPiece();
				GameObject piece_object = ObjectManager.Instance.GetObject(ObjectList.Piece);
				Piece piece = piece_object.GetComponent<Piece>();
				piece.Init(new Axis(x, y), piece_id);

				field_piece[x, y] = piece;
			}
		}

		Invoke("CheckAllPieces", 1f);
	}

	public void ExplodeCircle(int min_range = 2, int max_range = 4) {
		if(isAnimating || isRaining) {
			return;
		}
		is_score_reduced = true;

		int explode_random = (min_range >= max_range) ? min_range : Random.Range(min_range, max_range);
		Axis axis_explode = Axis.Random(X_MAX, Y_MAX);

		if(ListManager.Instance.mainRune == MainRune.TimeBreaker) {
			EffectManager.Instance.SetEffect(axis_explode, EffectList.TimeExplode, ListManager.Instance.ExplodeAudioClip, 1 + explode_random * 2, 1 + explode_random * 2);
		} else {
			EffectManager.Instance.SetEffect(axis_explode, EffectList.Explode, ListManager.Instance.ExplodeAudioClip, 1 + explode_random * 2, 1 + explode_random * 2);
		}

		for(int dy = -explode_random; dy <= explode_random; dy++) {
			for(int dx = -explode_random; dx <= explode_random; dx++) {
				Axis axis_explode_target = Axis.Add(axis_explode, new Axis(dx, dy));
				if(Axis.Available(axis_explode_target, X_MAX, Y_MAX)) {
					piece_remove_list.Add(AxisToPiece(axis_explode_target));
				}
			}
		}

		RemovePieces();
	}

	public void ExplodeAll(int min_range = -1, int max_range = -1) {
		if(isAnimating || isRaining) {
			return;
		}
		is_score_reduced = true;

		StartCoroutine(EffectManager.Instance.SetMainRuneEffect(MainRune.Opportunist));

		for(int y = 0; y < Y_MAX; y++) {
			for(int x = 0; x < X_MAX; x++) {
				Axis axis_explode_target = new Axis(x, y);
				if(Axis.Available(axis_explode_target, X_MAX, Y_MAX)) {
					piece_remove_list.Add(AxisToPiece(axis_explode_target));
				}
			}
		}

		RemovePieces();
	}

	void SetContinuableField() {
		bool isContinuableField = false;
		for(int y = 0; y < Y_MAX; y++) {
			for(int x = 0; x < X_MAX; x++) {
				Axis axis_target = new Axis(x, y);
				if(CheckAvailableCombo(axis_target)) {
					axis_hint = axis_target.Clone();
					isContinuableField = true;
					goto EXIT_FOR;
				}
			}
		}

		EXIT_FOR :
		if(isContinuableField) {
			//Debug.Log("Game Play is continuable");
		} else {
			//Debug.Log("NONE");
			reservedExplode = new Explode(ExplodeCircle);
			explode_min_range = 3; explode_max_range = 5;
		}
	}

	bool CheckAvailableCombo(Axis axis_check, Axis axis_dir) {
		byte[,] field_piece_main = new byte[X_MAX, Y_MAX];
		InitFieldClone();

		Axis axis_target = Axis.Add(axis_check, axis_dir);
		if(Axis.Available(axis_target, X_MAX, Y_MAX)) {
			byte temp = field_piece_main[axis_check.x, axis_check.y];
			field_piece_main[axis_check.x, axis_check.y] = field_piece_main[axis_target.x, axis_target.y];
			field_piece_main[axis_target.x, axis_target.y] = temp;
					
			for(int i = 0; i < 4; i++) {
				List<Axis> piece_combo = new List<Axis>();
				DepthFirstSearch(piece_combo, axis_check, i);
				if(piece_combo.Count > 2) {
					//Debug.Log($"{axis_check.x}, {axis_check.y}");
					return true;
				}
			}
		}

		return false;

		void InitFieldClone() {
			for(int y = 0; y < Y_MAX; y++) {
				for(int x = 0; x < X_MAX; x++) {
					field_piece_main[x, y] = field_piece[x, y].GetID();
				}
			}
		}

		void DepthFirstSearch(List<Axis> piece_combo, Axis axis, int direction, int depth = 1) {
			piece_combo.Add(axis);

			Axis axis_target_r = Axis.Add(axis, Axis.DIRECTION[direction]);
			if(Axis.Available(axis_target_r, X_MAX, Y_MAX)) {
				if(field_piece_main[axis.x, axis.y] == field_piece_main[axis_target_r.x, axis_target_r.y]) {
					DepthFirstSearch(piece_combo, axis_target_r, direction, depth + 1);
				}
			}

			if(depth == 1) {
				Axis axis_target_l = Axis.Add(axis, Axis.DIRECTION[7-direction]);
				if(Axis.Available(axis_target_l, X_MAX, Y_MAX)) {
					if(field_piece_main[axis.x, axis.y] == field_piece_main[axis_target_l.x, axis_target_l.y]) {
						DepthFirstSearch(piece_combo, axis_target_l, 7-direction, depth + 1);
					}
				}
			}
		}
	}

	bool CheckAvailableCombo(Axis axis_check) {
		for(int r = 0; r < 4; r++) {
			Axis axis_dir = r switch {
				0 => Axis.DIRECTION[1],
				1 => Axis.DIRECTION[3],
				2 => Axis.DIRECTION[4],
				3 =>Axis.DIRECTION[6],
			};

			if(CheckAvailableCombo(axis_check, axis_dir)) {
				return true;
			}
		}

		return false;
	}

	void RainingField() {
		if(formingPossibleField) {
			invokePossibleFieldDenied = true;
		}
		formingPossibleField = true;

		for(int x = 0; x < X_MAX; x++) {
			for(int y = Y_MAX - 2; y >= 0 ; y--) {
				if(field_piece[x, y] != null && field_piece[x, y + 1] == null) {
					for(int y_target = y + 1; y_target < Y_MAX; y_target++) {
						if(field_piece[x, y_target] == null && (y_target == Y_MAX - 1 || field_piece[x, y_target + 1] != null)) {
							Piece piece = field_piece[x, y];
							if(piece != null) {
								field_piece[x, y_target] = piece;
								field_piece[x, y] = null;

								piece.Move(new Axis(x, y_target));
							}
						}
					}
				}
			}

			int piece_short_num = 0;
			for(int y = 0; y < Y_MAX; y++) {
				if(field_piece[x, y] == null) {
					piece_short_num++;
				}
			}

			if(piece_short_num > 0) {
				for(int r = 0; r < piece_short_num; r++) {
					byte piece_id = ListManager.Instance.GetRandomPiece();
					GameObject piece_object = ObjectManager.Instance.GetObject(ObjectList.Piece);
					Piece piece = piece_object.GetComponent<Piece>();
					piece.Init(new Axis(x, r), piece_id);

					field_piece[x, r] = piece;
				}
			}
		}

		Invoke("CheckAllPieces", 0.5f);
	}

	public IEnumerator MovePiece(Piece piece, Axis axis_dir) {
		if(isAnimating || isRaining) {
			yield break;
		}
		var wfs = new WaitForSeconds(0.35f);

	 	if(formingPossible) {
			invokePossibleDenied = true;
		}
		formingPossible = true;

		Axis axis_origin = piece.Axis;
		Axis axis_target = Axis.Add(axis_origin, axis_dir);
		
		if(Axis.Available(axis_target, X_MAX, Y_MAX)) {
			Piece piece_target = AxisToPiece(axis_target);

			isAnimating = true;

			piece_target.Move(axis_origin);
			piece.Move(axis_target);

			yield return wfs;

			if(CheckAvailableCombo(axis_origin, axis_dir) || CheckAvailableCombo(axis_target, new Axis(axis_origin.x - axis_target.x, axis_origin.y - axis_target.y))) {
				UnsetHint();

				combo_chain = 1;

				field_piece[axis_target.x, axis_target.y] = piece;
				field_piece[axis_origin.x, axis_origin.y] = piece_target;

				CheckPieces(piece.Axis, piece_target.Axis);
			} else {
				piece_target.Move(axis_target);
				piece.Move(axis_origin);

				audioSource.clip = ListManager.Instance.DeniedAudioClip;
				audioSource.Play();

				Invoke("SetPlayPossible", 0.3f);
			}
		}

		yield break;
	}

	void CheckAllPieces() {
		for(int y = 0; y < Y_MAX; y++) {
			for(int x = 0; x < X_MAX; x++) {
				CheckPiece(new Axis(x, y));
			}
		}

		RemovePieces();
	}

	void CheckPieces(params Axis[] axis_check_list) {
		for(int i = 0; i < axis_check_list.Length; i++) {
			CheckPiece(axis_check_list[i]);
		}

		RemovePieces();
	}

	void CheckPiece(Axis axis_check) {
		List<PieceCombo> pieceCombo_list = new List<PieceCombo>(); 
		for(int i = 0; i < 4; i++) {
			PieceCombo piece_combo = new PieceCombo();
			DepthFirstSearch(piece_combo, axis_check, i);
			pieceCombo_list.Add(piece_combo);
		}

		for(int i = 0; i < pieceCombo_list.Count; i++) {
			if(pieceCombo_list[i].GetListCount() > 2) {
				List<Piece> piece_list = pieceCombo_list[i].GetList();

				for(int j = 0; j < piece_list.Count; j++) {
					if(!piece_remove_list.Contains(piece_list[j])) {
						piece_remove_list.Add(piece_list[j]);
					}
				}
			}
		}

		void DepthFirstSearch(PieceCombo piece_combo, Axis axis, int direction, int depth = 1) {
			Piece piece = AxisToPiece(axis);
			piece_combo.GetList().Add(piece);

			Axis axis_target_r = Axis.Add(axis, Axis.DIRECTION[direction]);
			if(Axis.Available(axis_target_r, X_MAX, Y_MAX)) {
				Piece piece_target = AxisToPiece(axis_target_r);
				if(piece != null && piece_target != null && piece.GetID() == piece_target.GetID()) {
					DepthFirstSearch(piece_combo, axis_target_r, direction, depth + 1);
				}
			}

			if(depth <= 1) {
				Axis axis_target_l = Axis.Add(axis, Axis.DIRECTION[7-direction]);
				if(Axis.Available(axis_target_l, X_MAX, Y_MAX)) {
					Piece piece_target = AxisToPiece(axis_target_l);
					if(piece != null && piece_target != null && piece.GetID() == piece_target.GetID()) {
						DepthFirstSearch(piece_combo, axis_target_l, 7-direction, depth + 1);
					}
				}
			}
		}
	}

	void RemovePieces() {
		for(int i = piece_remove_list.Count - 1; i >= 0; i--) {
			RemovePiece(piece_remove_list[i]);
		}

		if(piece_remove_list.Count > 0) {
			SetScore(piece_remove_list);
		}

		if(piece_remove_list.Count > 0) {
			isRaining = true;
			Invoke("RainingField", 0.3f);
		} else {
			SetPlayPossibleField();
		}

		piece_remove_list.Clear();
	}

	void RemovePiece(Piece piece_target) {
		Axis axis_target = piece_target.Axis;

		field_piece[axis_target.x, axis_target.y] = null;

		piece_target.RemoveAnimation();
	}

	void SetPlayPossible() {
		//if(!invokePossibleFieldDenied) {
			isAnimating = false;
			formingPossible = false;
		//} else {
		//	invokePossibleFieldDenied = false;
		//}
	}

	void SetPlayPossibleField() {
		//if(!invokePossibleFieldDenied) {
			SetContinuableField();
			isAnimating = false;
			isRaining = false;
			formingPossibleField = false;

			if(reservedExplode != null) {
				//if(ListManager.Instance.mainRune == MainRune.TimeBreaker) {
				//	reservedExplode(2, 4);
				//} else {
					reservedExplode(explode_min_range, explode_max_range);
				//}
				reservedExplode = null;
			}
		//} else {
		//	invokePossibleFieldDenied = false;
		//}
		
	}

	public void SetPause() {
		if(using_pause < 3) {
			if(is_paused) {
				PauseOut();
			} else {
				PauseIn();
			}
		}
	}

	void PauseIn() {
		ListManager.Instance.StopBGM();
		Time.timeScale = 0f;
		is_paused = true; 
		pause_content_txt.text = $"<size=100>{ListManager.GetLocalizedString("Score")}</size>\n{score_target}";
		pause_panel.SetActive(true);
		//user_interface_anim.SetTrigger("Pause");
	}

	void PauseOut() {
		ListManager.Instance.ContinueBGM();
		Time.timeScale = 1f;
		is_paused = false;
		pause_panel.SetActive(false);
		using_pause++;
		//user_interface_anim.SetTrigger("PauseOut");
	}

	void SaveData() {
		Data data = DataManager.Load();
		
		if(score_target > data.GetHighScore()) {
			data.longHighscore = score_target;

			if(Social.localUser.authenticated) {
				if(score_target >= 1_000_000_000) {
					GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_1000000000_score);
				}
				if(score_target >= 500_000_000) {
					GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_500000000_score);
				}
				if(score_target >= 100_000_000) {
					GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_100000000_score);
				}
				if(score_target >= 50_000_000) {
					GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_50000000_score);
				}
				if(score_target >= 10_000_000) {
					GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_10000000_score);
				}
				if(score_target >= 5_000_000) {
					GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_5000000_score);
				}
				if(score_target >= 1_000_000) {
					GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_1000000_score);
				}
				if(score_target >= 500_000) {
					GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_500000_score);
				}
				if(score_target >= 100_000) {
					GPGSManager.Instance.UnlockAchievement(GPGSIds.achievement_100000_score);
				}
			}
		}
		data.longLastscore = score_target;
		data.Coin += coin;

		DataManager.Save(data);
	}

	void SetCaution(string caution_str) {
        caution_txt.text = caution_str;

        caution_panel.SetActive(true);
    }

    public void UnsetCaution() {
        caution_panel.SetActive(false);
    }

	public Transform AxisToTrasform(Axis axis) {
		return field_object.transform.Find($"{axis.x},{axis.y}");
	}

	public Vector3 AxisToVector(Axis axis) {
		return AxisToTrasform(axis).GetComponent<RectTransform>().anchoredPosition;
	}

	public Piece AxisToPiece(Axis axis) {
		return field_piece[axis.x, axis.y];
	}
}

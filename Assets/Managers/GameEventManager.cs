public static class GameEventManager {
	
	public delegate void GameEvent();
	
	public static event GameEvent GameStart, GameOver, DayStart, NightStart;
	
	public static void TriggerGameStart(){
		if(GameStart != null){
			GameStart();
		}
	}
	
	public static void TriggerGameOver(){
		if(GameOver != null){
			GameOver();
		}
	}

	public static void TriggerDayStart(){
		if(DayStart != null){
			DayStart();
		}
	}

	public static void TriggerNightStart(){
		if(NightStart != null){
			NightStart();
		}
	}
}
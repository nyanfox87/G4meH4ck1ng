import pyautogui
import threading
import time
from pynput import keyboard

# 設定 pyautogui
pyautogui.PAUSE = 0

# 全域變數控制
clicking = False
exit_program = False

def clicker():
    """負責執行點擊邏輯的執行緒"""
    global clicking
    
    print("--- 點擊器已就緒 (按下 'e' 開始/停止，按下 'Esc' 結束程式) ---")
    
    while not exit_program:
        if clicking:
            start_time = time.perf_counter()
            duration = 20  # 持續 20 秒
            
            print(">> 開始點擊...")
            
            # 在 20 秒內且 clicking 為 True 時持續點擊
            while time.perf_counter() - start_time < duration and clicking:
                pyautogui.click()
                # 為了達到約 50 CPS，我們微調 sleep 時間
                # 1/50 = 0.02s，但程式執行本身有耗時，故設更小或不設
                time.sleep(0.01) 
            
            if clicking:
                print(">> 時間到，自動停止。")
                clicking = False
        
        time.sleep(0.1)  # 待命狀態下的 CPU 保護

def on_press(key):
    """監聽按鍵事件"""
    global clicking, exit_program
    
    try:
        if key.char == 'e':
            clicking = not clicking
            status = "啟動" if clicking else "手動停止"
            print(f"狀態變更: {status}")
    except AttributeError:
        # 處理非字母按鍵 (如 Esc)
        if key == keyboard.Key.esc:
            print("正在結束程式...")
            clicking = False
            exit_program = True
            return False

# 啟動點擊執行緒
click_thread = threading.Thread(target=clicker)
click_thread.start()

# 啟動鍵盤監聽
with keyboard.Listener(on_press=on_press) as listener:
    listener.join()
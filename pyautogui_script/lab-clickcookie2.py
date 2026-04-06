import pyautogui
import threading
import time
from pynput import keyboard
from PIL import ImageGrab

# 初始化設定
pyautogui.PAUSE = 0
clicking = False
exit_program = False

def check_color_pause():
    """
    檢查當前滑鼠位置顏色是否為 (R > 100, G=0, B=0)
    返回 True 表示符合該顏色，應暫停點擊
    """
    try:
        x, y = pyautogui.position()
        # 取得滑鼠座標點的顏色 (RGB)
        # 注意：ImageGrab 在多螢幕或高解析度縮放(DPI)下可能需要調整
        rgb = ImageGrab.grab(bbox=(x, y, x + 1, y + 1)).getpixel((0, 0))
        r, g, b = rgb
        if r > 100 and g == 0 and b == 0:
            return True
    except Exception:
        pass
    return False

def clicker_loop():
    """核心點擊執行緒"""
    global clicking, exit_program
    
    print("--- 點擊器已就緒 ---")
    print("操作說明：按 'e' 開始/停止，按 'Esc' 退出。")
    print("條件偵測：當鼠標指向純紅色 (R>100, G=0, B=0) 時會自動暫停。")

    while not exit_program:
        if clicking:
            start_time = time.perf_counter()
            duration = 20  # 持續 20 秒
            
            print(">> [啟動] 點擊中...")
            
            while time.perf_counter() - start_time < duration and clicking:
                # 顏色判斷邏輯
                if check_color_pause():
                    # 顏色符合 (紅色)，進入等待迴圈直到顏色改變或點擊被手動關閉
                    while check_color_pause() and clicking:
                        time.sleep(0.05) # 降低 CPU 負擔
                    continue
                
                # 執行點擊
                pyautogui.click()
                
                # 50 CPS 頻率控制 (1/50 = 0.02s)
                # 扣除程式邏輯與顏色偵測耗時，通常設為 0.01s ~ 0.015s 較接近 50 CPS
                time.sleep(0.01) 
            
            if clicking:
                print(">> [結束] 20 秒時間到。")
                clicking = False
        
        time.sleep(0.1)

def on_press(key):
    global clicking, exit_program
    try:
        if key.char == 'e':
            clicking = not clicking
            state = "開始" if clicking else "停止"
            print(f"目前狀態：{state}")
    except AttributeError:
        if key == keyboard.Key.esc:
            exit_program = True
            return False

# 啟動執行緒
thread = threading.Thread(target=clicker_loop, daemon=True)
thread.start()

# 鍵盤監聽
with keyboard.Listener(on_press=on_press) as listener:
    listener.join()
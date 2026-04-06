import pyautogui
import math
import time
import threading
from pynput import keyboard

# 初始化設定
pyautogui.PAUSE = 0
drawing_active = False  # 控制繪圖線程的開關
stop_drawing = False    # 強制中斷標誌

def draw_circle():
    global drawing_active, stop_drawing
    drawing_active = True
    stop_drawing = False
    
    # 取得當前鼠標位置作為圓心
    center_x, center_y = pyautogui.position()
    radius = 200
    segments = 400
    
    print(f"\n[系統] 開始繪圖：圓心 ({center_x}, {center_y})")

    try:
        # 1. 移動到起始點 (角度為 0 的位置)
        start_x = center_x + radius * math.cos(0)
        start_y = center_y + radius * math.sin(0)
        pyautogui.moveTo(start_x, start_y)

        # 2. 開始循環繪製細分邊
        for i in range(1, segments + 1):
            if stop_drawing:
                print("[系統] 偵測到 Q 鍵，繪圖已強制中斷。")
                break
            
            # 計算下一個點的弧度
            angle = (2 * math.pi / segments) * i
            target_x = center_x + radius * math.cos(angle)
            target_y = center_y + radius * math.sin(angle)
            
            # 執行拖曳：持續時間 0.01秒，間隔延遲 0.05秒
            pyautogui.dragTo(target_x, target_y, duration=0.01)
            time.sleep(0.05)
        
        # 3. 結束後執行一次 click
        if not stop_drawing:
            pyautogui.click()
            print("[系統] 繪製完成並已執行點擊。")

    except Exception as e:
        print(f"[錯誤] 繪圖過程發生問題: {e}")
    finally:
        drawing_active = False

def on_press(key):
    global stop_drawing
    try:
        if hasattr(key, 'char'):
            if key.char == 's':
                if not drawing_active:
                    # 開啟異步線程繪圖
                    thread = threading.Thread(target=draw_circle)
                    thread.daemon = True
                    thread.start()
                else:
                    print("[警告] 繪圖正在進行中...")
            
            elif key.char == 'q':
                stop_drawing = True
            
            elif key.char == 'x':
                print("[系統] 程式關閉中...")
                return False # 停止監聽器
    except AttributeError:
        pass

# 啟動程式
print("=== 高速自動繪圖助手 ===")
print("控制說明：")
print("  [s] - 以目前鼠標位置為圓心開始繪圖")
print("  [q] - 立即中斷當前繪圖作業")
print("  [x] - 關閉程式")
print("------------------------")
print("狀態：等待輸入中...")

with keyboard.Listener(on_press=on_press) as listener:
    listener.join()
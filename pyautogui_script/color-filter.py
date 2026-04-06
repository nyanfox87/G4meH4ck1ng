import tkinter as tk
from tkinter import ttk
import pyautogui
from PIL import Image, ImageTk, ImageDraw
import colorsys
from pynput import keyboard
import threading

class ColorDropperTool:
    def __init__(self, root):
        self.root = root
        self.root.title("Python 滴管工具")
        self.root.geometry("280x450")
        self.root.resizable(False, False)
        
        # 放大鏡設定
        self.MAGNIFIER_SIZE = 150  # 放大鏡顯示大小 (畫素)
        self.ZOOM_FACTOR = 5       # 放大倍率
        # 計算實際需要抓取的螢幕區域大小
        self.GRAB_SIZE = self.MAGNIFIER_SIZE // self.ZOOM_FACTOR 
        
        # 當前狀態變數
        self.current_rgb = (0, 0, 0)
        self.current_pos = (0, 0)
        self.recorded_colors = []
        
        # 建立 UI 元件
        self._setup_ui()
        
        # 啟動鍵盤監聽執行緒 (避免阻塞主執行緒)
        self.listener = keyboard.Listener(on_press=self._on_key_press)
        self.listener.start()
        
        # 啟動即時更新循環
        self.update_loop()

    def _setup_ui(self):
        # 1. 放大鏡區域
        self.magnifier_frame = ttk.Frame(self.root, padding=10)
        self.magnifier_frame.pack(side=tk.TOP, fill=tk.X)
        
        # 用於顯示放大圖像的 Label
        self.canvas_label = ttk.Label(self.magnifier_frame)
        self.canvas_label.pack()
        
        # 2. 資訊顯示區域
        self.info_frame = ttk.LabelFrame(self.root, text=" 顏色資訊 ", padding=10)
        self.info_frame.pack(side=tk.TOP, fill=tk.BOTH, expand=True, padx=10, pady=5)
        
        # 座標
        ttk.Label(self.info_frame, text="座標 (X, Y):").grid(row=0, column=0, sticky=tk.W)
        self.pos_label = ttk.Label(self.info_frame, text="0, 0", font=("Consolas", 10, "bold"))
        self.pos_label.grid(row=0, column=1, sticky=tk.W, padx=5)
        
        # HEX
        ttk.Label(self.info_frame, text="HEX:").grid(row=1, column=0, sticky=tk.W, pady=2)
        self.hex_label = ttk.Label(self.info_frame, text="#000000", font=("Consolas", 10, "bold"))
        self.hex_label.grid(row=1, column=1, sticky=tk.W, padx=5)
        
        # RGB
        ttk.Label(self.info_frame, text="RGB:").grid(row=2, column=0, sticky=tk.W, pady=2)
        self.rgb_label = ttk.Label(self.info_frame, text="0, 0, 0", font=("Consolas", 10, "bold"))
        self.rgb_label.grid(row=2, column=1, sticky=tk.W, padx=5)
        
        # HSV
        ttk.Label(self.info_frame, text="HSV:").grid(row=3, column=0, sticky=tk.W, pady=2)
        self.hsv_label = ttk.Label(self.info_frame, text="0°, 0%, 0%", font=("Consolas", 10, "bold"))
        self.hsv_label.grid(row=3, column=1, sticky=tk.W, padx=5)
        
        # 顏色預覽小方塊
        self.color_preview = tk.Frame(self.info_frame, width=30, height=30, relief=tk.SOLID, borderwidth=1)
        self.color_preview.grid(row=1, column=2, rowspan=2, padx=10)

        # 3. 記錄區域
        self.record_frame = ttk.LabelFrame(self.root, text=" 已記錄顏色 (按 'C' 記錄) ", padding=5)
        self.record_frame.pack(side=tk.TOP, fill=tk.BOTH, expand=True, padx=10, pady=10)
        
        self.record_listbox = tk.Listbox(self.record_frame, height=5, font=("Consolas", 9))
        self.record_listbox.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)
        
        scrollbar = ttk.Scrollbar(self.record_frame, orient=tk.VERTICAL, command=self.record_listbox.yview)
        scrollbar.pack(side=tk.RIGHT, fill=tk.Y)
        self.record_listbox.config(yscrollcommand=scrollbar.set)

    def rgb_to_hex(self, rgb):
        return "#{:02x}{:02x}{:02x}".format(rgb[0], rgb[1], rgb[2])

    def rgb_to_hsv(self, rgb):
        # colorsys 需要 0.0 ~ 1.0 的值
        r, g, b = [x / 255.0 for x in rgb]
        h, s, v = colorsys.rgb_to_hsv(r, g, b)
        # 轉回常用單位：H(0-360), S(0-100%), V(0-100%)
        return (int(h * 360), int(s * 100), int(v * 100))

    def update_loop(self):
        """即時更新放大鏡和資訊的核心循環"""
        # 取得目前滑鼠座標
        x, y = pyautogui.position()
        self.current_pos = (x, y)
        
        # 1. 螢幕擷取與放大邏輯
        try:
            # 計算擷取區域，使滑鼠位於中心
            left = x - self.GRAB_SIZE // 2
            top = y - self.GRAB_SIZE // 2
            
            # 使用 ImageGrab 抓取一小塊區域 (這比抓全螢幕快很多)
            screenshot = pyautogui.screenshot(region=(left, top, self.GRAB_SIZE, self.GRAB_SIZE))
            
            # 取得中心點顏色
            center_pixel = (self.GRAB_SIZE // 2, self.GRAB_SIZE // 2)
            # 有時 pyautogui 抓取的 screenshot 與 PIL 格式不完全相容，確保是 RGB
            self.current_rgb = screenshot.convert('RGB').getpixel(center_pixel)
            
            # 放大圖像
            magnified_img = screenshot.resize((self.MAGNIFIER_SIZE, self.MAGNIFIER_SIZE), Image.NEAREST)
            
            # 在中心畫一個十字準心
            draw = ImageDraw.Draw(magnified_img)
            center = self.MAGNIFIER_SIZE // 2
            cross_len = 5
            # 根據背景顏色決定準心顏色 (反色)
            cross_color = tuple(255 - c for c in self.current_rgb)
            draw.line((center - cross_len, center, center + cross_len, center), fill=cross_color, width=1)
            draw.line((center, center - cross_len, center, center + cross_len), fill=cross_color, width=1)
            
            # 將 PIL 圖像轉為 Tkinter 可用的 PhotoImage
            self.tk_image = ImageTk.PhotoImage(magnified_img)
            self.canvas_label.config(image=self.tk_image)
            
        except Exception as e:
            # 處理滑鼠移出螢幕邊界等例外
            print(f"擷取更新錯誤: {e}")

        # 2. 更新文字資訊
        self._update_info_text(x, y)
        
        # 3. 顏色預覽
        current_hex = self.rgb_to_hex(self.current_rgb)
        self.color_preview.config(bg=current_hex)
        
        # 4. 安排下一次更新 (約 30 FPS)
        self.root.after(33, self.update_loop)

    def _update_info_text(self, x, y):
        self.pos_label.config(text=f"{x}, {y}")
        
        hex_val = self.rgb_to_hex(self.current_rgb)
        self.hex_label.config(text=hex_val)
        
        self.rgb_label.config(text=f"{self.current_rgb[0]}, {self.current_rgb[1]}, {self.current_rgb[2]}")
        
        h, s, v = self.rgb_to_hsv(self.current_rgb)
        self.hsv_label.config(text=f"{h}°, {s}%, {v}%")

    def _on_key_press(self, key):
        """pynput 的鍵盤監聽回調"""
        try:
            # 如果按下的是字母 'c'
            if key.char == 'c':
                self._record_color()
        except AttributeError:
            # 忽略特殊按鍵 (Shift, Ctrl 等)
            pass

    def _record_color(self):
        """記錄當前顏色到清單"""
        x, y = self.current_pos
        rgb = self.current_rgb
        hex_val = self.rgb_to_hex(rgb)
        
        record_entry = f"{hex_val} | RGB: {rgb[0]},{rgb[1]},{rgb[2]} | At:({x},{y})"
        
        # 使用 root.after 確保對 UI 的修改回到 Tkinter 的主執行緒執行
        self.root.after(0, lambda: self._update_listbox(record_entry))

    def _update_listbox(self, entry):
        self.record_listbox.insert(tk.END, entry)
        self.record_listbox.see(tk.END) # 自動滾動到底部
        print(f"已記錄: {entry}")

if __name__ == "__main__":
    root = tk.Tk()
    app = ColorDropperTool(root)
    
    # 確保關閉視窗時，監聽執行緒也會結束
    def on_closing():
        app.listener.stop()
        root.destroy()
        
    root.protocol("WM_DELETE_WINDOW", on_closing)
    root.mainloop()
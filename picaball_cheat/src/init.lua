print('hello')

local app = require('src.config.app')

local process = app.process
assert(process, 'Missing process name in config: expected app.process or app.picaball.process')

print(process)

local function getPID()
    local processes = getProcesslist()
    local pid = nil

    for pid, name in pairs(processes) do
        if name == process then
            return pid
        end
    end
    return nil
end

local function autoAttach()
    local pid = getPID()
    if pid then
        -- print('Auto-attaching to process: ' .. process .. ' with PID: ' .. pid)
        openProcess(pid)
        attached = true
    else
        -- print('Process not found for auto-attach: ' .. process)
        pid = nil
        if not attached then
            return
        end
        
        window.destroy()
    end
end

local function createWindow()
    local title = app.window.title or 'Cheat Window'
    local width = app.window.width or 800
    local height = app.window.height or 600
    window = createForm(nil)
    window.setCaption(title)
    window.setWidth(width)
    window.setHeight(height)
    attached = false

    local MainForm = getMainForm()
    local attachTimer = createTimer(MainForm)

    attachTimer.setInterval(1000) -- Check every second
    attachTimer.setOnTimer(autoAttach)

    window.show()
end

createWindow()
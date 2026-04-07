local MODULE_PATH = [[;.\src\?.lua]]

local function isTrainer()
    local appName = getApplication().ExeName
    return appName and appName:lower():find('cheatengine')
end

if isTrainer() then
    if not string.endsWith(package.path, MODULE_PATH) then
        package.path = package.path .. MODULE_PATH
    end
end

local app = require('config.app')
local window = require('views.window')
local menu = require('views.menu')

trainer = {
    attached = false,
    pid = nil,
}

local function getPID()
    local processes = getProcesslist()

    for pid, name in pairs(processes) do
        if name == app.client.picaball.name then
            return pid
        end
    end
    return nil
end

local function detachTrainer()
    if not trainer.attached then
        return
    end

    trainer.attached = false
    trainer.pid = nil
    menu.detach()
end

local function attachTrainer(pid)
    if trainer.attached and trainer.pid == pid then
        return
    end

    trainer.attached = true
    trainer.pid = pid
    openProcess(pid)
    menu.attach()
end

local function autoAttach()
    local pid = getPID()

    if not pid then
        detachTrainer()
    else
        attachTrainer(pid)
    end
end

local function create()
    window.draw()
    menu.draw()

    local mainForm = getMainForm()
    local attachTimer = createTimer(mainForm)

    attachTimer.setInterval(1000)
    attachTimer.setOnTimer(autoAttach)

    window.show()
end

create()
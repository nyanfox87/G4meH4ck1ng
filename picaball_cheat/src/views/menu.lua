local util = require('util')
local navigate = require('config.navigate')
local pointer = require('config.pointer')

local Menu = createMainMenu(Window)

local function prompt(title, spec)
    util.prompt(title, spec)
end

local function syncMenuStatus(sender, list, ref)
    for i = 0, sender.count - 1 do
        local menuItem = sender.Item[i]
        local nav = list[i + 1]

        if #nav > 0 then
            syncMenuStatus(menuItem, nav, ref[nav.key])
        else
            local spec = ref[nav.key]
            local count = util.getPointerValue(spec)

            if nav.type == 'prompt' then
                menuItem.caption = nav.caption .. '\t' .. tostring(count)
            end
        end
    end
end

local function createMenu(owner, list, ref)
    local isMain = owner.className == 'TMainMenu'

    for _, value in ipairs(list) do
        local menuItem = createMenuItem(value)
        menuItem.setCaption(value.caption)

        if isMain then
            owner.Items.add(menuItem)
            menuItem.enabled = value.enabled
            menuItem.onClick = function(sender)
                syncMenuStatus(sender, value, ref[value.key])
            end
        else
            owner.add(menuItem)
        end

        if value.type and ref then
            local spec = ref[value.key]
            if value.type == 'prompt' then
                menuItem.onClick = function()
                    prompt(value.caption, spec)
                end
            end
        end

        createMenu(menuItem, value, ref[value.key] or {})
    end
end

local function draw()
    createMenu(Menu, navigate, pointer)
end

local function attach()
    for index = 0, Menu.Items.count - 1 do
        Menu.Items[index].enabled = true
    end
end

local function detach()
    for index = 0, Menu.Items.count - 1 do
        Menu.Items[index].enabled = navigate[index + 1].enabled
    end
end

return {
    draw = draw,
    attach = attach,
    detach = detach,
}

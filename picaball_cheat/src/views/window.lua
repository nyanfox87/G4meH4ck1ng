local app = require('config.app')

Window = createForm(false)

local function draw()
    Window.setCaption(app.window.title)
    Window.setWidth(app.window.width)
    Window.setHeight(app.window.height)
    Window.setColor(clWhite)
    Window.setOnClose(closeCE)
end

local function show()
    Window.centerScreen()
    Window.show()
end

return {
    draw = draw,
    show = show,
}

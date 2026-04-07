local app = require('config.app')

local function resolveAddress(spec)
    local processName = app.client.picaball.name
    local baseAddress = getAddress(processName .. '+' .. string.format('%X', spec.base))
    local pointer = readPointer(baseAddress)

    for i = 1, #spec.chain - 1 do
        pointer = readPointer(pointer + spec.chain[i])
    end

    return pointer + spec.chain[#spec.chain]
end

local function getPointerValue(spec)
    local address = resolveAddress(spec)
    return readInteger(address)
end

local function setPointerValue(spec, value)
    local address = resolveAddress(spec)
    writeInteger(address, value)
end

local function prompt(title, spec)
    local address = resolveAddress(spec)
    local currentValue = readInteger(address)
    local input = inputQuery(title, 'Enter new value:', currentValue)

    if input ~= nil then
        writeInteger(address, tonumber(input) or currentValue)
    end

    return input
end

return {
    getPointerValue = getPointerValue,
    setPointerValue = setPointerValue,
    prompt = prompt,
}

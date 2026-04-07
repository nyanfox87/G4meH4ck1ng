import pymem
import pymem.process

# 1. SETUP: Change these to match your Cheat Engine finding
PROCESS_NAME = "picaball.exe"
# The pointer path: [BaseOffset, Offset1, Offset2, ...]
MY_POINTER = [0x11F20, 0xD48, 0xC, 0x38, 0xDAC] 

# 2. ATTACH
try:
    pm = pymem.Pymem(PROCESS_NAME)
    # Get the "picaball.exe" base address (the green address in CE)
    module_base = pymem.process.module_from_name(pm.process_handle, PROCESS_NAME).lpBaseOfDll
    print(f"Connected to {PROCESS_NAME}!")
except:
    print("Error: Game not found.")
    exit()

# 3. INTERACT
while True:
    try:
        new_value = int(input("Enter new value (or type 9999 to quit): "))
        if new_value == 9999: break

        # Step-by-step Pointer Chasing
        # Start at the base
        addr = pm.read_int(module_base + MY_POINTER[0])
        
        # Follow the middle offsets
        for offset in MY_POINTER[1:-1]:
            addr = pm.read_int(addr + offset)
            
        # Add the final offset to get the destination
        final_addr = addr + MY_POINTER[-1]
        
        # Write the value
        pm.write_int(final_addr, new_value)
        print(f"Successfully changed value to: {new_value}")

    except Exception as e:
        print(f"Write failed: {e}. Are you sure you are in a match?")
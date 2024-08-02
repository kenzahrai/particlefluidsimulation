extends Button

var paused = false
# Called when the node enters the scene tree for the first time.
func _ready():
	pressed.connect(self._button_pressed)
	pass # Replace with function body.


# Called every frame. 'delta' ipauseds the elapsed time since the previous frame.

func _button_pressed():
	paused = !paused
	if paused:
		text = "resume"
	else:
		text = "pause"


extends Button

var done = false
# Called when the node enters the scene tree for the first time.
func _ready():
	pressed.connect(self._button_pressed)
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.

func _button_pressed():
	get_parent().get_parent().AddParticle()

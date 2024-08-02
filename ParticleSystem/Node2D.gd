extends Node2D


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _draw():
	draw_circle(Vector2.ZERO,1,Color(255,255,255,0.5))
	pass

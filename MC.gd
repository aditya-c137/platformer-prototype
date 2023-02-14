extends KinematicBody2D


export(Resource) var player_props = preload("res://DefaultPlayer.tres")

#calculated player parameters
var GRAVITY: float
var JUMP_VELOCITY: float
var MAX_VELOCITY_X: float

var velocity = Vector2.ZERO

func _ready():
	calculate_gravity_and_velocity()

func _physics_process(delta):
	apply_gravity(delta)
	var input = Vector2.ZERO
	input.x = Input.get_axis("ui_left", "ui_right")
	move_player(input, delta)
	
	velocity = move_and_slide(velocity, Vector2.UP)

func move_player(input, delta):
	if input.x != 0:
		velocity.x = MAX_VELOCITY_X * input.x
#		apply_acceleration(input.x, delta)         #TODO: advanced horizontal movement
	else:
		velocity.x = 0
#		apply_friction(delta)                      #TODO: advanced horizontal movement
	
	if Input.is_action_just_pressed("ui_up"):
		velocity.y = -JUMP_VELOCITY
	

func apply_gravity(delta):
	velocity.y += GRAVITY
#	velocity.y = min(velocity.y, player_props.MAX_FALL_VELOCITY)    #FIX_ME: find the appropriate value for this 

func apply_friction(delta):
	velocity.x = move_toward(velocity.x, 0, player_props.DECELERATION * delta)

func apply_acceleration(amount, delta):
	velocity.x = move_toward(velocity.x , MAX_VELOCITY_X * amount, player_props.ACCELERATION * delta)

func calculate_gravity_and_velocity():
	GRAVITY = 8 * player_props.JUMP_HEIGHT / pow(player_props.JUMP_TIME, 2)
	JUMP_VELOCITY = 4 * player_props.JUMP_HEIGHT / player_props.JUMP_TIME
	MAX_VELOCITY_X = player_props.JUMP_RANGE * player_props.JUMP_TIME
	print(GRAVITY)
	print(JUMP_VELOCITY)
	print(MAX_VELOCITY_X)

class Car {
	constructor(vec, speed) {
		this.position = vec;
		this.directionRadians = 45;
		this.speed = speed;
	}

	speedVector() {
		let t = p5.Vector.fromAngle(
			radians(this.directionRadians * oneDegreeInRadians),
			this.speed
		);

		return t;
	}

	move() {
		rect(this.position.x, this.position.y, 50, 30);
		this.position.add(this.speedVector());
	}
}

let x = 10;
const oneDegreeInRadians = 0.0174533;
let car = null;

function setup() {
	createCanvas(800, 400);

	car = new Car(createVector(200, 200), 2);
	car2 = new Car(createVector(100, 40), 3);
	car3 = new Car(createVector(10, 10), 4);
}

function draw() {
	background(220);

	car.move();
	car2.move();
	car3.move();
}

class StatAllocation {
	constructor() {
		this.HP = 0;
		this.MP = 0;
		this.ATK = 0;
		this.DEF = 0;
		this.SPD = 0;
		this.MATK = 0;
	}
}

const statAllocation = new StatAllocation();
var buttonReset = document.getElementById('stat-reset');
var buttonApply = document.getElementById('stat-apply');

function getPointsForStat(statName) {
	switch (statName) {
		case 'HP': case 'ATK': return unAllocatedPoints.ExercisePoints;
		case 'MP': case 'MATK': return unAllocatedPoints.StudyPoints;
		case 'DEF': case 'SPD': return unAllocatedPoints.HouseworkPoints;
		default: return 0;
	}
}

function isContainsStat() {
	return Object.values(statAllocation).some(value => value > 0);
}

function updateButtons() {
	document.querySelectorAll('[data-stat]').forEach(button => {
		const statName = button.getAttribute('data-stat');
		button.disabled = getPointsForStat(statName) <= 0;
	});

	// ボタンの有効・無効を更新
	if (buttonReset != null) {
		buttonReset.disabled = !isContainsStat();
	}
	if (buttonApply != null) {
		buttonApply.disabled = !isContainsStat();
	}
}

function updatePointsDisplay() {
	document.getElementById('exercise-points').textContent =
		unAllocatedPoints.ExercisePoints;
	document.getElementById('study-points').textContent =
		unAllocatedPoints.StudyPoints;
	document.getElementById('housework-points').textContent =
		unAllocatedPoints.HouseworkPoints;
}

function resetPoints() {
	Object.entries(statAllocation).forEach(([statName, value]) => {
		switch (statName) {
			case 'HP':
			case 'ATK':
				unAllocatedPoints.ExercisePoints += value;
				break;
			case 'MP':
			case 'MATK':
				unAllocatedPoints.StudyPoints += value;
				break;
			case 'DEF':
			case 'SPD':
				unAllocatedPoints.HouseworkPoints += value;
				break;
		}

		// リセット後は0に戻す
		statAllocation[statName] = 0;
	});
}

document.querySelectorAll('[data-stat]').forEach(button => {
	button.addEventListener('click', () => {
		const statName = button.getAttribute('data-stat');

		switch (statName) {
			case 'HP':
			case 'ATK':
				unAllocatedPoints.ExercisePoints--;
				break;
			case 'MP':
			case 'MATK':
				unAllocatedPoints.StudyPoints--;
				break;
			case 'DEF':
			case 'SPD':
				unAllocatedPoints.HouseworkPoints--;
				break;
		}
		statAllocation[statName]++;
		updateButtons();
		updatePointsDisplay();
	});
});

if (buttonReset && buttonApply) {

	buttonReset.addEventListener('click', () => {
		resetPoints();
		updateButtons();
		updatePointsDisplay();
	});

	buttonApply.addEventListener('click', async () => {
		const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
		const response = await fetch('/api/character/allocate', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				'RequestVerificationToken': token
			},
			body: JSON.stringify(statAllocation)
		});
		location.reload();
	});
}



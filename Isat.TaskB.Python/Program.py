def Precision(T, P):
	return T / P;

def Recall(T, C):
	return T / C;

def FMeasure(T, C, P):
	if (T == 0):
		return 0;
	precision = Precision(T, P);
	recall = Recall(T, C);
	return 2 * precision * recall / (precision + recall);

def PrecisionW(T, C, P):
	return T * C / P;

classesCount = int(input());
classes = [[]];
all = 0;

for i in range(classesCount):
	iClassString = input();
	iClassStringList = iClassString.split();
	classes.append([]);
	for numberString in iClassStringList:
		number = int(numberString);
		classes[i].append(number);
		all += number;

Ts = [];
Cs = [];
Ps = [];

for i in range(classesCount):
	Ci = 0;
	Pi = 0;
	for j in range(classesCount):
		Ci += classes[i][j];
		Pi += classes[j][i];
	Ts.append(classes[i][i]);
	Cs.append(Ci);
	Ps.append(Pi);

precisionW = 0;
recallW = 0;
microF = 0;

for i in range(classesCount):
	if (Cs[i] != 0 and Ps[i] != 0):
		precisionW += PrecisionW(Ts[i], Cs[i], Ps[i]);
		recallW += Ts[i];
		microF += Cs[i] * FMeasure(Ts[i], Cs[i], Ps[i]);

precisionW /= all;
recallW /= all;
microF /= all;

macroF = 2 * precisionW * recallW / (precisionW + recallW);

print(macroF);
print(microF);
from sklearn.ensemble import AdaBoostClassifier
import os
import pandas
from sklearn.model_selection import train_test_split
import matplotlib.pyplot as pyplot

def colors (labels):
    return ["gold" if label == "P" else "deepskyblue" for label in labels]

fileName = "geyser"

data = pandas.read_csv(fileName + ".csv")

targetValues = data["class"].values
attributes = data.drop(["class"], axis = 1).values

attributesTrain, attributesTest, targetValuesTrain, targetValueTest = train_test_split(attributes, targetValues, test_size = 0.1)

bestAccuracy, bestLearningRate, bestIteration = 0, 0, 0

for i in range (0, 100):
    learningRate = 1e-3 * 1.1 ** i
    model = AdaBoostClassifier(learning_rate = learningRate, n_estimators = 100)
    model.fit(attributesTrain, targetValuesTrain)

    targetValuePredictedFunctions = model.staged_predict(attributes)
    scoreFunctions = model.staged_score(attributesTest, targetValueTest)

    for j, item in enumerate(scoreFunctions, start = 0):
        if (item > bestAccuracy):
            bestAccuracy = item
            bestLearningRate = learningRate
            bestIteration = j

print(bestLearningRate)
print(bestAccuracy)
print(bestIteration)

model = AdaBoostClassifier(learning_rate = bestLearningRate, n_estimators = 100)
model.fit(attributesTrain, targetValuesTrain)

targetValuePredictedFunctions = model.staged_predict(attributes)
scoreFunctions = model.staged_score(attributesTest, targetValueTest)

iterationsCount = []
accuracies = []

os.mkdir(fileName)

for i, item in enumerate(targetValuePredictedFunctions, start = 1):
    pyplot.xlabel("Step number " + str(i))
    pyplot.scatter(attributes[:, 0], attributes[:, 1], c = colors(targetValues), cmap = pyplot.cm.Paired)
    ax = pyplot.gca()
    ax.scatter(attributes[:, 0], attributes[:, 1], facecolors = "none", edgecolors = colors(item), s = 100, linewidths = 1)
    pyplot.savefig(fileName + "/" + "iteration" + str(i) + ".png")

for i, item in enumerate(scoreFunctions, start = 0):
    iterationsCount.append(i)
    accuracies.append(item)

pyplot.clf()
pyplot.plot(iterationsCount, accuracies, label = "")
pyplot.xlabel("n estimators")
pyplot.ylabel("Accuracy")
pyplot.legend()
pyplot.show()
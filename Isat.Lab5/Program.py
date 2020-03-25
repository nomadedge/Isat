from sklearn.ensemble import AdaBoostClassifier
import os
import pandas
import numpy
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
    classifier = AdaBoostClassifier(learning_rate = learningRate, n_estimators = 100)
    classifier.fit(attributesTrain, targetValuesTrain)

    targetValuePredictedFunctions = classifier.staged_predict(attributes)
    scoreFunctions = classifier.staged_score(attributesTest, targetValueTest)

    for j, item in enumerate(scoreFunctions, start = 0):
        if (item > bestAccuracy):
            bestAccuracy = item
            bestLearningRate = learningRate
            bestIteration = j

print(bestLearningRate)
print(bestAccuracy)
print(bestIteration)

classifier = AdaBoostClassifier(learning_rate = bestLearningRate, n_estimators = 100)
classifier.fit(attributesTrain, targetValuesTrain)

targetValuePredictedFunctions = classifier.staged_predict(attributes)
scoreFunctions = classifier.staged_score(attributesTest, targetValueTest)

pyplot.scatter(attributes[:, 0], attributes[:, 1], c = colors(targetValues), cmap = pyplot.cm.Paired)

axes = pyplot.gca()
xLim = axes.get_xlim()
yLim = axes.get_ylim()

xs = numpy.linspace(xLim[0], xLim[1], 30)
ys = numpy.linspace(yLim[0], yLim[1], 30)

xCoords, yCoords = numpy.meshgrid(xs, ys)

xyCoords = numpy.vstack([xCoords.ravel(), yCoords.ravel()]).T

Z = classifier.staged_decision_function(xyCoords)

iterationsCount = []
accuracies = []

os.mkdir(fileName)

i = 1
for item, z in zip(targetValuePredictedFunctions, Z):
    pyplot.clf()

    pyplot.xlabel("Step number " + str(i))
    pyplot.scatter(attributes[:, 0], attributes[:, 1], c = colors(targetValues), cmap = pyplot.cm.Paired)

    z = z.reshape(xCoords.shape)

    axes = pyplot.gca()

    axes.contour(xCoords, yCoords, z, colors = "black", levels = [-1, 0, 1], linestyles = ["--", "-", "--"])

    axes.scatter(attributes[:, 0], attributes[:, 1], facecolors = "none", edgecolors = colors(item), s = 100, linewidths = 1)
    pyplot.savefig(fileName + "/" + "iteration" + str(i) + ".png")

    i += 1

for i, item in enumerate(scoreFunctions, start = 0):
    iterationsCount.append(i)
    accuracies.append(item)

pyplot.clf()
pyplot.plot(iterationsCount, accuracies, label = "")
pyplot.xlabel("n estimators")
pyplot.ylabel("Accuracy")
pyplot.legend()
pyplot.show()
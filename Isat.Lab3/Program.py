import matplotlib.pyplot as pyplot
import numpy
import pandas
import sys
from sklearn.model_selection import cross_val_score
from sklearn.preprocessing import normalize
from sklearn.svm import SVC

fileName = "chips.csv"
#fileName = "geyser.csv"

def colors (labels):
    return ["gold" if label == "P" else "deepskyblue" for label in labels]

cs = [1e-2, 1e-1, 1e0, 1e1, 1e2, 1e3, 1e4]
kernels = ["linear", "poly", "rbf", "sigmoid"]

data = pandas.read_csv(fileName)

targetValues = data["class"].values
attributes = data.drop(["class"], axis = 1).values

normalizedAttributes = normalize(attributes)
factorizedTargetValues = pandas.factorize(data["class"])[0]

maxMean = sys.float_info.min
bestC = 0
bestKernel = ""

for c in cs:
    for kernel in kernels:
        classifier = SVC(C = c, kernel = kernel)
        scores = cross_val_score(classifier, normalizedAttributes, factorizedTargetValues, scoring="f1")
        print(kernel + " " + str(c) + " " + str(scores.mean()))
        if scores.mean() > maxMean:
            maxMean = scores.mean()
            bestC = c
            bestKernel = kernel

print(bestKernel + " " + str(bestC))
bestClassifier = SVC(kernel = bestKernel, C = bestC)
bestClassifier.fit(attributes, targetValues)

pyplot.scatter(attributes[:, 0], attributes[:, 1], c = colors(targetValues), cmap = pyplot.cm.Paired)

axes = pyplot.gca()
xLim = axes.get_xlim()
yLim = axes.get_ylim()

xs = numpy.linspace(xLim[0], xLim[1], 70)
ys = numpy.linspace(yLim[0], yLim[1], 70)

xCoords, yCoords = numpy.meshgrid(xs, ys)

xyCoords = numpy.vstack([xCoords.ravel(), yCoords.ravel()]).T
Z = bestClassifier.decision_function(xyCoords).reshape(xCoords.shape)

axes.contour(xCoords,
             yCoords,
             Z,
             colors = "black",
             levels = [-1, 0, 1],
             linestyles = ["--", "-", "--"])

axes.scatter(bestClassifier.support_vectors_[:, 0],
             bestClassifier.support_vectors_[:, 1],
             s = 100,
             linewidth = 1,
             facecolors = "none",
             edgecolors = "black")

pyplot.show()
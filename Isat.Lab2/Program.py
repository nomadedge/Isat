import numpy
from sklearn.linear_model import SGDRegressor, Ridge, LinearRegression, RANSACRegressor
from sklearn import preprocessing
import matplotlib.pyplot as pyplot

def smape(A, F):
    return 100/len(A) * numpy.sum(2 * numpy.abs(F - A) / (numpy.abs(A) + numpy.abs(F)))

def _error(actual: numpy.ndarray, predicted: numpy.ndarray):
    return actual - predicted

def mse(actual: numpy.ndarray, predicted: numpy.ndarray):
    return numpy.mean(numpy.square(_error(actual, predicted)))

def rmse(actual: numpy.ndarray, predicted: numpy.ndarray):
    return numpy.sqrt(mse(actual, predicted))

def nrmse(actual: numpy.ndarray, predicted: numpy.ndarray):
    return rmse(actual, predicted) / (actual.max() - actual.min())

#read from file and normalize

xTraining = []
yTraining = []
xTest = []
yTest = []

with open ("files/1.txt", "r") as datasetFile:
    data = datasetFile.read()
dataStrings = data.split("\n")

attributesCount = int(dataStrings[0])
trainingCount = int(dataStrings[1])

for i in range(2, 2 + trainingCount):
    attributeStrings = dataStrings[i].split(" ")
    attributes = []
    for j in range(0, attributesCount):
        attributes.append(int(attributeStrings[j]))
    xTraining.append(attributes)
    yTraining.append(attributeStrings[attributesCount])

testCount = int(dataStrings[2 + trainingCount])
xTraining = preprocessing.normalize(xTraining)

for i in range(3 + trainingCount, 3 + trainingCount + testCount):
    attributeStrings = dataStrings[i].split(" ")
    attributes = []
    for j in range(0, attributesCount):
        attributes.append(int(attributeStrings[j]))
    xTest.append(attributes)
    yTest.append(attributeStrings[attributesCount])

xTest = preprocessing.normalize(xTest)
yTraining, yTest = numpy.asarray(yTraining, dtype=numpy.float), numpy.array(yTest, dtype=numpy.float)
xTraining = numpy.hstack((xTraining, numpy.ones((xTraining.shape[0], 1))))
xTest = numpy.hstack((xTest, numpy.ones((xTest.shape[0], 1))))

#mnk with svd

modelRidge = Ridge(alpha=0.5, solver='svd')
modelRidge.fit(xTraining, yTraining)
yPredictedRidge = modelRidge.predict(xTest)
errorTest = nrmse(yPredictedRidge, yTest)
print(errorTest)

#sgd

#sgdIterations, sgdError = [], []

for i in range(10, 501, 10):
    modelSgd = SGDRegressor(shuffle=True, max_iter=i, penalty="elasticnet", alpha=0.01, learning_rate="invscaling", eta0=0.001, l1_ratio=0.6, power_t=0.3)
    modelSgd.fit(xTraining, yTraining)
    yPredictedSgd = modelSgd.predict(xTest)
    sgdIterations.append(i)
    sgdNrmse = nrmse(yPredictedSgd, yTest)
    sgdError.append(sgdNrmse)
    print(i)
    print(sgdNrmse)
    print()

pyplot.plot(sgdIterations, sgdError, label="NRMSE - iterations count dependency")
pyplot.xlabel("Iterations count")
pyplot.ylabel("NRMSE")
pyplot.legend()
pyplot.show()

#ransac

ransacIterations, ransacError = [], []

for i in range(1, 11, 1):
    modelRansac = RANSACRegressor(max_trials=i, max_skips=100)
    modelRansac.fit(xTraining, yTraining)
    yPredictedRansac = modelRansac.predict(xTest)
    ransacIterations.append(i)
    ransacNrmse = nrmse(yPredictedRansac, yTest)
    ransacError.append(ransacNrmse)
    print(i)
    print(ransacNrmse)
    print()

pyplot.plot(ransacIterations, ransacError, label="NRMSE - Trials count dependency")
pyplot.xlabel("Trials count")
pyplot.ylabel("NRMSE")
pyplot.legend()
pyplot.show()
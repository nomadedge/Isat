import sys
from sklearn.metrics import confusion_matrix
import keras
from keras.datasets import fashion_mnist, mnist
from keras.layers import Dense, Activation, Flatten, Conv2D, MaxPooling2D
from keras.models import Sequential
from keras.utils import to_categorical
import numpy
import matplotlib.pyplot as pyplot
import pandas
import tensorflow
import seaborn

(trainX, trainY), (testX, testY) = mnist.load_data()
testXOriginal = testX
trainX = trainX.reshape(-1, 28, 28, 1)
testX = testX.reshape(-1, 28, 28, 1)
trainX = trainX.astype('float32')
testX = testX.astype('float32')
trainX = trainX / 255
testX = testX / 255
trainYOneHot = to_categorical(trainY)
testYOneHot = to_categorical(testY)

optimizers = ['adam', 'adagrad', 'adadelta', 'adamax', 'nadam']
bestLoss = sys.float_info.max
bestOptimizer = 'sgd'

for optimizer in optimizers:
    model = Sequential()

    model.add(Conv2D(64, (3, 3), input_shape=(28, 28, 1)))
    model.add(Activation('relu'))
    model.add(MaxPooling2D(pool_size=(2, 2)))

    model.add(Conv2D(64, (3, 3)))
    model.add(Activation('relu'))
    model.add(MaxPooling2D(pool_size=(2, 2)))

    model.add(Flatten())
    model.add(Dense(64))

    model.add(Dense(10))
    
    model.add(Activation('softmax'))
    model.compile(loss=keras.losses.categorical_crossentropy, optimizer=optimizer, metrics=['accuracy'])
    model.fit(trainX, trainYOneHot, batch_size=64, epochs=2)
    testLoss, testAccuracy = model.evaluate(testX, testYOneHot)
    print('Optimizer ', optimizer)
    print('Test loss', testLoss)
    print('Test accuracy', testAccuracy)
    if testLoss < bestLoss:
        bestLoss = testLoss
        bestOptimizer = optimizer

print('Best optimizer is {} with loss {}'.format(bestOptimizer, bestLoss))
model.compile(loss=keras.losses.categorical_crossentropy, optimizer=bestOptimizer, metrics=['accuracy'])
model.fit(trainX, trainYOneHot, batch_size=64, epochs=10)
classes = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
predictions = model.predict(testX)
testYPredicted = []
for i in range(10000):
    testYPredicted.append(numpy.argmax(predictions[i]))
print(confusion_matrix(testY, testYPredicted))

#now calculate with best parameters on fashion_mnist

(trainX, trainY), (testX, testY) = fashion_mnist.load_data()
testXOriginal = testX
trainX = trainX.reshape(-1, 28, 28, 1)
testX = testX.reshape(-1, 28, 28, 1)
trainX = trainX.astype('float32')
testX = testX.astype('float32')
trainX = trainX / 255
testX = testX / 255
trainYOneHot = to_categorical(trainY)
testYOneHot = to_categorical(testY)

model = Sequential()

model.add(Conv2D(64, (3, 3), input_shape=(28, 28, 1)))
model.add(Activation('relu'))
model.add(MaxPooling2D(pool_size=(2, 2)))

model.add(Conv2D(64, (3, 3)))
model.add(Activation('relu'))
model.add(MaxPooling2D(pool_size=(2, 2)))

model.add(Flatten())
model.add(Dense(64))

model.add(Dense(10))

model.add(Activation('softmax'))

print('Best optimizer is {} with loss {}'.format(bestOptimizer, bestLoss))
model.compile(loss=keras.losses.categorical_crossentropy, optimizer=bestOptimizer, metrics=['accuracy'])
model.fit(trainX, trainYOneHot, batch_size=64, epochs=10)
classes = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
predictions = model.predict(testX)
testYPredicted = []
for i in range(10000):
    testYPredicted.append(numpy.argmax(predictions[i]))
print(confusion_matrix(testY, testYPredicted))

for i in range(10000):
    if testYPredicted[i] != testY[i]:
        pyplot.imshow(testXOriginal[i])
        pyplot.text(1, 1, s=str(testY[i])+" | "+str(testYPredicted[i]))
        pyplot.show()
        pyplot.clf()
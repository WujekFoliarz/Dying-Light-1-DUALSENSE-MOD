// pch.h: wstępnie skompilowany plik nagłówka.
// Wymienione poniżej pliki są kompilowane tylko raz, co poprawia wydajność kompilacji dla przyszłych kompilacji.
// Ma to także wpływ na wydajność funkcji IntelliSense, w tym uzupełnianie kodu i wiele funkcji przeglądania kodu.
// Jednak WSZYSTKIE wymienione tutaj pliki będą ponownie kompilowane, jeśli którykolwiek z nich zostanie zaktualizowany między kompilacjami.
// Nie dodawaj tutaj plików, które będziesz często aktualizować (obniża to korzystny wpływ na wydajność).

#ifndef PCH_H
#define PCH_H

// w tym miejscu dodaj nagłówki, które mają być wstępnie kompilowane
#include "framework.h"
#include <vector>
#include <iostream>
#include <chrono>
#include <thread>
#include "DualSenseAPI.h"
#include "ViGEm/Client.h"
#pragma comment(lib, "setupapi.lib")
#include <cmath>
#include <stdexcept>
#include "Vigem.h"
#include <cstdlib> 
#include <ctime>  
#include <atomic>

#endif //PCH_H

cmake_minimum_required (VERSION 3.8)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Enable Hot Reload for MSVC compilers if supported.
if (POLICY CMP0141)
  cmake_policy(SET CMP0141 NEW)
  set(CMAKE_MSVC_DEBUG_INFORMATION_FORMAT "$<IF:$<AND:$<C_COMPILER_ID:MSVC>,$<CXX_COMPILER_ID:MSVC>>,$<$<CONFIG:Debug,RelWithDebInfo>:EditAndContinue>,$<$<CONFIG:Debug,RelWithDebInfo>:ProgramDatabase>>")
endif()

project (libScePadHapticExtension)

file(GLOB_RECURSE MY_SOURCES CONFIGURE_DEPENDS "${CMAKE_CURRENT_SOURCE_DIR}/source/*.cpp")

set(COMPILE_TO_EXE OFF CACHE BOOL "Compile to exe" FORCE) # Delete the out folder when you change this

if(COMPILE_TO_EXE)
add_executable(${PROJECT_NAME} ${MY_SOURCES})
target_compile_definitions(${PROJECT_NAME} PUBLIC COMPILE_TO_EXE=1)
else()
add_library(${PROJECT_NAME} SHARED ${MY_SOURCES})
target_compile_definitions(${PROJECT_NAME} PUBLIC COMPILE_TO_EXE=0)
endif()

set_target_properties(${PROJECT_NAME} PROPERTIES POSITION_INDEPENDENT_CODE ON)
target_include_directories(${PROJECT_NAME}
  PUBLIC
    "${CMAKE_CURRENT_SOURCE_DIR}/include"
    "${CMAKE_CURRENT_SOURCE_DIR}/source"
)

target_compile_definitions(${PROJECT_NAME} PRIVATE SCEPADHAPTICS_EXPORTS)

target_link_libraries(${PROJECT_NAME} PRIVATE duaLib) # Replace libScePad with duaLib if you use that
cmake_minimum_required (VERSION 3.8)

set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Release>:Release>")
set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")

# Enable Hot Reload for MSVC compilers if supported.
if (POLICY CMP0141)
  cmake_policy(SET CMP0141 NEW)
  set(CMAKE_MSVC_DEBUG_INFORMATION_FORMAT "$<IF:$<AND:$<C_COMPILER_ID:MSVC>,$<CXX_COMPILER_ID:MSVC>>,$<$<CONFIG:Debug,RelWithDebInfo>:EditAndContinue>,$<$<CONFIG:Debug,RelWithDebInfo>:ProgramDatabase>>")
endif()

project (DyingLightScePad)
file(GLOB_RECURSE MY_SOURCES CONFIGURE_DEPENDS "${CMAKE_CURRENT_SOURCE_DIR}/source/*.cpp")

add_library(${PROJECT_NAME} SHARED ${MY_SOURCES})

set_target_properties(${PROJECT_NAME} PROPERTIES POSITION_INDEPENDENT_CODE ON)
target_include_directories(${PROJECT_NAME}
  PUBLIC
    "${CMAKE_CURRENT_SOURCE_DIR}/include"
    "${CMAKE_CURRENT_SOURCE_DIR}/source"
)

# Subdirectiories
# add_subdirectory(thirdparty/)
add_subdirectory(thirdparty/minhook)
add_subdirectory(thirdparty/duaLib)
add_subdirectory(thirdparty/libScePad-Haptic-Extension)

# Linking libraries
target_link_libraries(${PROJECT_NAME} PRIVATE minhook duaLib libScePadHapticExtension)

# Copying DLLs to out folder
# add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy_if_different $<TARGET_FILE:duaLib> $<TARGET_FILE_DIR:${PROJECT_NAME}>)

target_compile_features(${PROJECT_NAME} PUBLIC cxx_std_20)
add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND ${CMAKE_COMMAND} -E rename $<TARGET_FILE:${PROJECT_NAME}> $<TARGET_FILE_DIR:${PROJECT_NAME}>/$<TARGET_FILE_BASE_NAME:${PROJECT_NAME}>.asi)
